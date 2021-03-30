// Copyright © 2010 Microsoft Corporation, All Rights Reserved.
// This code released under the terms of the Microsoft Research License Agreement (MSR-LA, http://sds.codeplex.com/License)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Discovery.Data
{
    /// <summary>    
    /// TransactedVariable is a logical container for an arbitrary typed array and related metadata dictionary,
    /// that provides a mechanism of transactional change of its change.
    /// This class shall be used for variables those provide an access to data storages.
    /// </summary>
    /// <typeparam name="DataType">Type of data for the variable.</typeparam>
    public abstract class TransactedVariable<DataType> : Variable<DataType>
    {
        #region Private members

        private Changes changes;

        private CommitStage commitStage;

        private bool writeTransactionOpened = false;

        #endregion

        #region Public properties

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an instance of the variable.
        /// </summary>
        protected TransactedVariable(IDataSetProvider provider, string query, string name, string[] dims) :
            base(provider, query, name, dims)
        {
            commitStage = CommitStage.Committed;
            StartChanging();
        }

        /// <summary>
        /// Initializes an instance of the variable.
        /// </summary>
        /// <remarks>
        /// This constructor implies that variable's query is equal to its name.
        /// </remarks>
        protected TransactedVariable(IDataSetProvider provider, string name, string[] dims) :
            this(provider, name, name, dims)
        {
        }

        #endregion

        #region Events


        #endregion

        #region Metadata


        #endregion

        #region Reversing


        #endregion

        #region Get Data

        #endregion

        #region Put Data
        
        /// <summary>
        /// Writes the data as a part of transaction with id specified 
        /// to the variable's underlying storage starting with specified origin indices.
        /// *It is called in the precommit stage*
        /// </summary>
        /// <remarks>
        /// A sequence of such outputs with same transactionId can be either committed by CommitWrite() or
        /// rolled back by RollbackWrite().
        /// </remarks>
        /// <param name="origin">Indices to start adding of data. Null means all zeros.</param>
        /// <param name="a">Data to add to the variable.</param>
        protected abstract void WriteData(int[] origin, Array a);

        /// <summary>
        /// Begins new write-transaction and thus prepares to a sequence of WriteData.
        /// If the variable can support write-transactions it shall override this method.
        /// </summary>
        protected virtual void BeginWriteTransaction()
        {
        }

        /// <summary>
        /// The methods is called right after all data in transaction is written with WriteData()
        /// and before the call of Commit(). It might be overriden.
        /// </summary>
        protected virtual void OnWriteFinished()
        {
        }

        /// <summary>
        /// Commits a sequence of write-transactions.
        /// If the variable can support write-transactions it shall override this method.
        /// </summary>
        protected virtual void CommitWrite()
        {
        }

        /// <summary>
        /// Rolls back a sequence of write-transactions.
        /// If the variable can support write-transactions it shall override this method.
        /// </summary>
        protected virtual void RollbackWrite()
        {
        }

        /// <summary>
        /// Adds the data to the variable starting with specified origin indices.
        /// </summary>
        /// <param name="origin">Indices to start adding of data. Null means all zeros.</param>
        /// <param name="a">Data to add to the variable.</param>
        public override void PutData(int[] origin, Array a)
        {
            if (origin != null && origin.Length != Rank)
                throw new ArgumentException("Origin contains incorrect number of dimensions.");
            if (a.Rank != Rank)
                throw new ArgumentException("Array has wrong rank.");

            StartChanging();

            if (origin == null)
                origin = new int[Rank];

            // Updating shape
            DimensionList proposedDims = changes.Dimensions;
            if (proposedDims == null)
            {
                proposedDims = dims.Clone();
                changes.Dimensions = proposedDims;
            }

            for (int i = 0; i < proposedDims.Count; i++)
            {
                int shape = origin[i] + a.GetLength(i);
                if (shape > proposedDims[i].Length)
                    proposedDims[i] = new Dimension(proposedDims[i].Name, shape);
            }

            // Adding new data piece to the change list
            DataPiece piece = new DataPiece();
            piece.Origin = origin;
            piece.Data = a;

            changes.DataPieces.Add(piece);

            // Firing the event
            FireEventVariableChanged(VariableChangeAction.PutData);
        }

        #endregion

        #region Coordinate Systems and Dimensions

       
        #endregion

        #region Schemas and transactions

        /// <summary>
        /// Gets the value indicating whether the variable has been changed
        /// since last commit including schema changes or data update.
        /// </summary>
        public override bool HasChanges
        {
            get { return commitStage != CommitStage.Committed; }
        }

        /// <summary>
        /// Rolls back all changes made since last successful commit.
        /// </summary>
        public override void Rollback()
        {
            if (!HasChanges) return;

            if (writeTransactionOpened)
            {
                try
                {
                    RollbackWrite();
                    writeTransactionOpened = false;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Exception in rollback: " + ex.Message);
                }
            }

            ClearChanges();

            FireEventVariableRolledback();
        }

        /// <summary>
        /// Makes checks of internal constraints.
        /// </summary>
        internal override void CheckConstraints()
        {
            if (!HasChanges)
                return;
            if (commitStage >= CommitStage.ConstrainsChecked)
                return;

            CheckCoordinateSystems();
            OnCheckConstraints(changes);

            this.commitStage = CommitStage.ConstrainsChecked;
        }

        /// <summary>
        /// If the variable needs for some extra checks before commission this method must be overriden.
        /// It shall throw an exception in case of a check failure.
        /// </summary>
        protected virtual void OnCheckConstraints(Changes proposedChanges)
        {
        }

        /// <summary>
        /// Set of dimensions for each CS must be equal to the set of dimensions for the variable.
        /// </summary>
        private void CheckCoordinateSystems()
        {
            DimensionList dlist = changes.Dimensions == null ? dims : changes.Dimensions;
            CoordinateSystemCollection cscoll = changes.Cs == null ? csystems : changes.Cs;

            foreach (CoordinateSystem cs in cscoll)
            {
                if (dlist != cs.GetDimensions(SchemaVersion.Recent))
                    throw new Exception("Set of dimensions for each coordinate system must be equal to the set of dimensions for the variable.");
            }
        }

        /// <summary>
        /// Checks both internal and external constraints and, if successful,
        /// changes the state of the variable in accordance with requests (including data writing)
        /// </summary>
        internal override void Precommit()
        {
            if (!HasChanges)
                return;

            if (commitStage > CommitStage.Precommitting)
                return;

            try
            {
                /* Checking internal constraints */
                DoCheckConstraints();

                commitStage = CommitStage.Precommitting;

                /* Checking external constraints */
                if (!FireEventVariableCommitting(changes))
                    throw new Exception("Commission is cancelled.");

                /* To avoid duplicate execution of the following code,
                 * because it might be executed in Precommit of dependand DataSet as
                 * a part of the VariableCommitting event handling */
                if (commitStage > CommitStage.Precommitting)
                    return;

                /* Changing state of the variable */
                if (changes.DataPieces != null && changes.DataPieces.Count > 0)
                {
                    BeginWriteTransaction();
                    writeTransactionOpened = true;

                    while (changes.DataPieces.Count > 0)
                    {
                        DataPiece piece = changes.DataPieces[0];
                        WriteData(piece.Origin, piece.Data);

                        changes.DataPieces.RemoveAt(0);
                    }
                }
            }
            catch
            {
                /* If operation failed rolling back to "changed" state.
                 * Now it is possible either to Rollback to previous committed state or 
                 * make further changes to fix errors.*/
                commitStage = CommitStage.Changed;
                throw;
            }

            /* Success: this variable is ready for commission */
            commitStage = CommitStage.Precommitted;
        }

        /// <summary>
        /// This method is called after all variables has changed their state successfully and
        /// commits the state of the variable, so it cannot be rolled back any more.
        /// </summary>
        internal override void FinalCommit()
        {
            if (!HasChanges)
                return;

            if (commitStage < CommitStage.Precommitted)
                throw new ApplicationException("Cannot perform final commit because there was no precommit stage.");

            try
            {
                /* Committing changes in structure */
                if (!String.IsNullOrEmpty(changes.Name))
                    this.name = changes.Name;

                if (changes.Dimensions != null)
                    this.dims = changes.Dimensions;

                if (changes.Cs != null)
                    this.csystems = changes.Cs;

                /* Committing written data */
                CommitWrite();
                writeTransactionOpened = false;
            }
            catch (Exception ex)
            {
                /* Exception during final commission. It is too late to try to roll back changes. */
                Trace.WriteLine("Exception in final commit: " + ex.Message);
            }

            Changes appliedChanges = changes;
            ClearChanges();

            FireEventVariableCommitted(appliedChanges);
        }

        private void StartChanging()
        {
            if (commitStage == CommitStage.Committed)
            {
                changes = new Changes();
                changes.DataPieces = new List<DataPiece>();
                changes.InitialSchema = GetSchema();
                changes.Cs = null;
            }

            commitStage = CommitStage.Changed;
        }

        /// <summary>
        /// Gets a copy of the Variable that contains all changes made to it since last commission.
        /// </summary>
        /// <returns></returns>
        public override Variable GetChanges()
        {
            if (!HasChanges)
                return null;

            throw new NotImplementedException("Sorry, not yet...");
        }

        /// <summary>
        /// Gets specified version of the schema for the variable describing its structure.
        /// </summary>
        public override VariableSchema GetSchema(SchemaVersion version)
        {
            if (HasChanges)
            {
                if (version == SchemaVersion.Committed)
                    return changes.InitialSchema;

                // Making a schema for proposed version of the variable

                string name = changes.Name == null ? this.name : changes.Name;
                ReadOnlyDimensionList roDims = new ReadOnlyDimensionList(changes.Dimensions == null ? dims : changes.Dimensions);
                VariableMetadata metadata = this.metadata.Clone();

                CoordinateSystemCollection cs = changes.Cs == null ? csystems : changes.Cs;
                CoordinateSystemSchema[] csSchemas = new CoordinateSystemSchema[cs.Count];
                for (int i = 0; i < csSchemas.Length; i++)
                {
                    csSchemas[i] = cs[i].GetSchema();
                }

                VariableSchema schema = new VariableSchema(name,
                    TypeOfData,
                    roDims,
                    csSchemas,
                    metadata);
                return schema;
            }
            else
            {
                if (version == SchemaVersion.Proposed)
                    throw new Exception("Variable is commited and has no changes.");

                CoordinateSystemSchema[] csSchemas = new CoordinateSystemSchema[csystems.Count];
                for (int i = 0; i < csSchemas.Length; i++)
                {
                    csSchemas[i] = csystems[i].GetSchema();
                }

                VariableSchema schema = new VariableSchema(
                    name, TypeOfData,
                    new ReadOnlyDimensionList(dimensions),
                    csSchemas,
                    metadata.Clone());

                return schema;
            }
        }

        #region Nested types

        private void ClearChanges()
        {
            if (writeTransactionOpened)
                throw new ApplicationException("Write-transaction cannot be opened at this moment!");

            changes = new Changes();
            commitStage = CommitStage.Committed;
        }

        private enum CommitStage
        {
            Committed,
            Changed,
            ConstrainsChecked,
            Precommitting,
            Precommitted
        }

        #endregion

        #endregion
       
        #region IDisposable Members

        /// <summary>
        /// Disposes the variable.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion
    }    
}

