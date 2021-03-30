using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Renci.Wwt.Core.Frames
{
    public class FrameCollection : IList<Frame>
    {
        private IDictionary<string, Frame> _framesByName = new Dictionary<string, Frame>();

        private List<Frame> _frames = new List<Frame>();

        internal FrameCollection(WwtClient client)
        {
            var response = client.GetAllFrames();

            foreach (var element in response.Elements())
            {
                var frame = Frame.Create(client, null, element);
                this._frames.Add(frame);

                this.AddFrames(frame as ReferenceFrame);
            }
        }

        public Frame this[string name]
        {
            get
            {
                if (this._framesByName.ContainsKey(name))
                {
                    return this._framesByName[name];
                }

                return null;
            }
            set
            {
                throw new NotSupportedException("Frame cannot be set.");
            }
        }

        private void AddFrames(ReferenceFrame frame)
        {
            if (frame == null)
                return;

            this._framesByName.Add(frame.Name, frame);

            foreach (var childFrame in frame.Elements)
            {
                this.AddFrames(childFrame as ReferenceFrame);
            }
        }

        #region IList<Frame> Members

        public int IndexOf(Frame item)
        {
            return this._frames.IndexOf(item);
        }

        public void Insert(int index, Frame item)
        {
            this._frames.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this._frames.RemoveAt(index);
        }

        public Frame this[int index]
        {
            get
            {
                return this._frames[index];
            }
            set
            {
                this._frames[index] = value;
            }
        }

        #endregion

        #region ICollection<Frame> Members

        public void Add(Frame item)
        {
            this._frames.Add(item);
        }

        public void Clear()
        {
            this._frames.Clear();
        }

        public bool Contains(Frame item)
        {
            return this._frames.Contains(item);
        }

        public void CopyTo(Frame[] array, int arrayIndex)
        {
            this._frames.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this._frames.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(Frame item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<Frame> Members

        public IEnumerator<Frame> GetEnumerator()
        {
            return this._frames.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._frames.GetEnumerator();
        }

        #endregion
    }
}
