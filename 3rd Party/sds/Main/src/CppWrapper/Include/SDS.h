// Copyright © 2010 Microsoft Corporation, All Rights Reserved.
// This code released under the terms of the Microsoft Research License Agreement
#pragma once

#include <msclr\marshal.h>
#include <gcroot.h>
#include <tchar.h>
#include <string.h>
#include <typeinfo>
#include <Time.h>

namespace ScSData = Microsoft::Research::Science::Data;
using namespace msclr::interop;

typedef signed char sbyte;

namespace sds
{

	/***********************Enums*************************/
	enum DataType
	{
		DOUBLE,
		FLOAT,
		SHORT,
		INT,
		LONGLONG,
		USHORT,
		UINT,
		ULONGLONG,
		BYTE,
		SBYTE,
		DATETIME,
		STRING,
		BOOL,
		DOUBLEARR,
		FLOATARR,
		SHORTARR,
		INTARR,
		LONGLONGARR,
		USHORTARR,
		UINTARR,
		ULONGLONGARR,
		BYTEARR,
		SBYTEARR,
		DATETIMEARR,
		STRINGARR,
		BOOLARR
	};

	/*****************************************************/
	/*******************Global**Methods*******************/
	
	template <typename T> T * ToLink( array<T>^ inputArray)
	{
		T * result = new T[inputArray -> Length];
		for(int ind = 0; ind < inputArray ->Length; ind++)
		{
			result[ind] = inputArray[ind];
		}
		return result;
	}
	/*****************************************************/

	class CppDataSet;
	class CppVariable;
	class VariableElement;
	class MulDimIndex;
	class TimeConverter;

	//Multidimensional index class
	private class MulDimIndex
	{
	public:
		inline MulDimIndex(array<System::Int32>^ dimensionsLength);
		inline MulDimIndex(const MulDimIndex & mulDimIndex);
		inline cli::array<int> ^ Current(void);
		inline void Add1(void);
		inline bool CanAdd1(void);
	private:
		gcroot<cli::array<int> ^> _index;
		gcroot<cli::array<int> ^> _dimLength;
		int _dimensions;
	};
	
	MulDimIndex::MulDimIndex(array<System::Int32>^ dimensionsLength):
	_index(gcnew array<int>(dimensionsLength -> Length)),
	_dimLength(dimensionsLength)
	{
		cli::array<int> ^ indexArr = _index;
		this -> _dimensions = dimensionsLength -> Length;
		for(int ind = 0; ind < _index -> Length; ind++)
		{
			indexArr[ind] = 0;
		}
	}

	MulDimIndex::MulDimIndex(const MulDimIndex & mulDimIndex):
	_index(mulDimIndex._index),
	_dimLength(mulDimIndex._dimLength)
	{
		cli::array<int> ^ indexArr = _index;
		this -> _dimensions = mulDimIndex._dimensions;
	}

	bool MulDimIndex::CanAdd1(void)
	{
		cli::array<int> ^ indexArr = this -> _index;
		cli::array<int> ^ dimshapeArr = this -> _dimLength;
		for(int ind = 0; ind < indexArr -> Length; ind++)
		{
			if(indexArr[ind] < dimshapeArr[ind] - 1)
				return true;
		}
		return false;
	}

	void MulDimIndex::Add1(void)
	{
		cli::array<int> ^ indexArr = this -> _index;
		cli::array<int> ^ dimshapeArr = this -> _dimLength;
		int curDim = this -> _dimensions - 1;
		while(++indexArr[curDim] >= dimshapeArr[curDim])
		{
			if(curDim == 0)
			throw gcnew System::InvalidOperationException(gcnew System::String("Index can't be increased"));
			indexArr[curDim--] = 0;   
		}
	}

	cli::array<int> ^ MulDimIndex::Current(void)
	{
		return this->_index;
	}


	//Table that contains variable references
	private class VariableTable
	{
	private:
		static const int _variableIncreaseConstant = 2;
		static const int _variableStartCount = 20;
		CppVariable ** _variableArray;
		int _variableCount;
		int _variableTableSize;

		void VariableTable::_IncreaseSize()
		{
			CppVariable ** newVariable =
				new CppVariable*[_variableIncreaseConstant * _variableTableSize];
			for(int ind = 0; ind < _variableTableSize; ind++)
			{
				newVariable[ind] = _variableArray[ind];
			}
			int oldSize = _variableTableSize;
			_variableTableSize *= _variableIncreaseConstant;
			for(int ind = oldSize; ind < _variableTableSize; ind++)
			{
				newVariable[ind] = NULL;
			}
			delete[] _variableArray;
			_variableArray = newVariable;
		}
	public:
		void VariableTable::AddVariableRef(CppVariable * variable)
		{
			//No place in table -> we should increase it's size
			if(_variableTableSize - _variableCount <= 0)
			{
				_IncreaseSize();
			}
			_variableArray[_variableCount] = variable;
			_variableCount++;
		}

		VariableTable::VariableTable()
		{
			_variableArray = new CppVariable*[_variableStartCount];
			_variableTableSize = _variableStartCount;
			for(int ind = 0; ind < _variableTableSize; ind++)
			{
				_variableArray[ind] = NULL;
			}
			_variableCount = 0;
		}

		VariableTable::~VariableTable()
		{
			for(int ind = 0; ind < _variableTableSize; ind++)
			{
				if(_variableArray[ind] != NULL) delete _variableArray[ind];
			}
			delete[] _variableArray;
		}
	};

	class StringBuffer {
	private:
		int size;
		char *buffer;
	public:
		StringBuffer()
		{
			size = 0;
			buffer = NULL;
		}

		~StringBuffer()
		{
			if(buffer != NULL)
				delete[] buffer;
		}

		char * operator()()
		{
			return buffer;
		}

		char *Copy(const char *s)
		{
			int len = strlen(s);
			if(len + 1 > size) {
				if(buffer)
					delete[] buffer;
				buffer = new char[size = len + 1];
			}
    		strcpy_s(buffer, size, s);
			return buffer;
		}

		static char * CopyManagedString(System::String^ s)
		{
			marshal_context ^ context = gcnew marshal_context();
			const char * temp = context->marshal_as<const char *>(s);
			char* result = new char[strlen(temp)+1];
			strcpy(result, temp);
			delete context;
			return result;
		}
	};

	private class TypeConverter
	{
		public:
			static DataType TypeToEnum(System::Type ^ type)
			{
				if(type == System::Double::typeid)
					return DataType::DOUBLE;
				else if(type == System::Single::typeid)
					return DataType::FLOAT;
				else if(type == System::Int16::typeid)
					return DataType::SHORT;
				else if(type == System::Int32::typeid)
					return DataType::INT;
				else if(type == System::Int64::typeid)
					return DataType::LONGLONG;
				else if(type == System::UInt16::typeid)
					return DataType::USHORT;
				else if(type == System::UInt32::typeid)
					return DataType::UINT;
				else if(type == System::UInt64::typeid)
					return DataType::ULONGLONG;
				else if(type == System::Byte::typeid)
					return DataType::BYTE;
				else if(type == System::SByte::typeid)
					return DataType::SBYTE;
				else if(type == System::DateTime::typeid)
					return DataType::DATETIME;
				else if(type == System::String::typeid)
					return DataType::STRING;
				else if(type == System::Boolean::typeid)
					return DataType::BOOL;
				else if(type -> IsArray)
				{
					if(type -> GetElementType() == System::Double::typeid)
						return DataType::DOUBLEARR;
					else if(type -> GetElementType() == System::Single::typeid)
						return DataType::FLOATARR;
					else if(type -> GetElementType() == System::Int16::typeid)
						return DataType::SHORTARR;
					else if(type -> GetElementType() == System::Int32::typeid)
						return DataType::INTARR;
					else if(type -> GetElementType() == System::Int64::typeid)
						return DataType::LONGLONGARR;
					else if(type -> GetElementType() == System::UInt16::typeid)
						return DataType::USHORTARR;
					else if(type -> GetElementType() == System::UInt32::typeid)
						return DataType::UINTARR;
					else if(type -> GetElementType() == System::UInt64::typeid)
						return DataType::ULONGLONGARR;
					else if(type -> GetElementType() == System::Byte::typeid)
						return DataType::BYTEARR;
					else if(type -> GetElementType() == System::SByte::typeid)
						return DataType::SBYTEARR;
					else if(type -> GetElementType() == System::DateTime::typeid)
						return DataType::DATETIMEARR;
					else if(type -> GetElementType() == System::String::typeid)
						return DataType::STRINGARR;
					else if(type -> GetElementType() == System::Boolean::typeid)
						return DataType::BOOLARR;
					else
						throw gcnew System::ArgumentException(gcnew System::String("Invalid data type"));
				}
				else
					throw gcnew System::ArgumentException(gcnew System::String("Invalid data type"));
			}
	};

	public class TimeConverter
	{
	private:
		friend class CppVariable;
		static System::DateTime _Origin()
		{
			System::DateTime origin = System::DateTime(1970, 1, 1, 00, 00, 00,System::DateTimeKind::Utc);
			return origin;
		}

		static time_t _DateTimeToTimeT(System::DateTime dateTime, System::DateTime origin, System::TimeZone ^ timeZone)       
		{           
#ifndef _USE_32BIT_TIME_T
			long long longTime = System::Convert::ToInt64((timeZone -> ToUniversalTime(dateTime) - origin).TotalSeconds);
			return (time_t)longTime;
#else
			int intTime = System::Convert::ToInt32((timeZone -> ToUniversalTime(dateTime) - origin).TotalSeconds);
			return (time_t)intTime;
#endif
		} 
	public:   
		static System::DateTime TimeTToDateTime(time_t timeT)      
		{
#ifndef _USE_32BIT_TIME_T
			long long time = (long long)timeT;
			System::TimeZone ^ timeZone = System::TimeZone::CurrentTimeZone;
			return timeZone -> ToLocalTime(_Origin().AddSeconds(time));     
#else
			int time = (int)timeT;
			System::TimeZone ^ timeZone = System::TimeZone::CurrentTimeZone;
			return timeZone -> ToLocalTime(_Origin().AddSeconds(time));   
#endif   
		}         
		static time_t DateTimeToTimeT(System::DateTime dateTime)       
		{           
			return _DateTimeToTimeT(dateTime, _Origin(), System::TimeZone::CurrentTimeZone);
		} 
		static array<System::DateTime>^ TimeTarrToDateTimeArr(time_t * timeTarr, int count)      
		{
			System::DateTime origin = _Origin();
			System::TimeZone ^ timeZone = System::TimeZone::CurrentTimeZone;
			array<System::DateTime>^  dateTimeArr = gcnew array<System::DateTime>(count);
			for(int ind = 0; ind < count; ind++)
			{
#ifndef _USE_32BIT_TIME_T
				long long time = (long long)timeTarr[ind];
				dateTimeArr[ind] = timeZone -> ToLocalTime(origin.AddSeconds(time));
#else
				int time = (int)timeTarr[ind];
				dateTimeArr[ind] = timeZone -> ToLocalTime(origin.AddSeconds(time));
#endif
			}
			return dateTimeArr;       
		}         
		static time_t * DateTimeArrToTimeTarr(array<System::DateTime>^ dateTimeArr)       
		{           
			System::TimeZone ^ timeZone = System::TimeZone::CurrentTimeZone;
			System::DateTime origin = _Origin();
			time_t *  timeTarr = new time_t[dateTimeArr -> Length];
			for(int ind = 0; ind < dateTimeArr -> Length; ind++)
			{
#ifndef _USE_32BIT_TIME_T
				long long longTime = System::Convert::ToInt64((timeZone -> ToUniversalTime(dateTimeArr[ind]) - origin).TotalSeconds);
				timeTarr[ind] = (time_t)longTime;
#else
				int intTime = System::Convert::ToInt32((timeZone -> ToUniversalTime(dateTimeArr[ind]) - origin).TotalSeconds);
				timeTarr[ind] = (time_t)intTime;
#endif
			}
			return timeTarr;
		} 

		static time_t * DateTimeArrToTimeTarr(System::Array ^ dateTimeArr)       
		{           
			System::TimeZone ^ timeZone = System::TimeZone::CurrentTimeZone;
			System::DateTime origin = _Origin();
			time_t *  timeTarr = new time_t[dateTimeArr -> Length];
			for(int ind = 0; ind < dateTimeArr -> Length; ind++)
			{
#ifndef _USE_32BIT_TIME_T
				long long longTime = System::Convert::ToInt64((timeZone -> ToUniversalTime(System::Convert::ToDateTime(dateTimeArr -> GetValue(ind))) - origin).TotalSeconds);
				timeTarr[ind] = (time_t)longTime;
#else
				int intTime = System::Convert::ToInt32((timeZone -> ToUniversalTime(System::Convert::ToDateTime(dateTimeArr -> GetValue(ind))) - origin).TotalSeconds);
				timeTarr[ind] = (time_t)intTime;
#endif
			}
			return timeTarr;
		} 
	};

	public class CppVariable
	{
	private:
		gcroot<Microsoft::Research::Science::Data::Variable ^> _variable;
		gcroot<Microsoft::Research::Science::Data::DataSet ^> _dataset;
		gcroot<System::String ^> _name;
		DataType _type;

		friend class CppDataSet;
		template<typename T> inline T * _GetTs(int ** shape);
		template<typename T> inline T * _GetTs(int * shape, int * origin);
		inline char ** _GetStrings(int ** shape);
		inline char ** _GetStrings(int * shape, int * origin);
		inline time_t * _GetDateTimes(int ** shape);
		inline time_t * _GetDateTimes(int * shape, int * origin);
		template<typename T> inline void _PutTs(T * Values, int * shape);
		inline void _PutStrings(char ** Values, int * shape);
		inline void _PutDateTimes(time_t * Values, int * shape);
		//MetaData
		template <typename T> inline T _GetScalarAtt(const char * name);
		template <typename T> inline void _PutScalarAtt(const char * name, T value);
		template <typename T> inline T * _GetAtt(const char * name, int * length);
		template <typename T> inline void _PutAtt(const char * name, T * value, int length);
		template <typename T> inline void _Append(T value);
		template <typename T> inline void _Append(T value, const char* dim);
		template <typename T> inline void _Append(T * values, int valuesRank, int * shape);
		template <typename T> inline void _Append(T * values, int valuesRank, int * shape, const char * dim);
		inline void _AppendString(char * value);
		inline void _AppendString(char * value, const char * dim );
		inline void _AppendStrings( char ** values, int valuesRank, int * shape);
		inline void _AppendStrings( char ** values, int valuesRank, int * shape, const char * dim);
		inline void _AppendDateTime(time_t value);
		inline void _AppendDateTime(time_t value,const char * dim );
		inline void _AppendDateTimes( time_t * values, int valuesRank, int * shape);
		inline void _AppendDateTimes( time_t * values, int valuesRank, int * shape, const char * dim);
		inline void _InitType();
	public:
		inline CppVariable(void);
		inline CppVariable(const char * name, CppDataSet * dataset);
		inline CppVariable(const char * name, ScSData::Variable ^ Variable);
		inline ~CppVariable(void);
		//Metadata methods
		/*Put*Scalar*/
		inline void PutAttShort(const char * name, short value);
		inline void PutAttInt(const char * name, int value);
		inline void PutAttLongLong(const char * name, long long value);
		inline void PutAttUnsignedShort(const char * name, unsigned short value);
		inline void PutAttUnsignedInt(const char * name, unsigned int value);
		inline void PutAttUnsignedLongLong(const char * name, unsigned long long value);
		inline void PutAttFloat(const char * name, float value);
		inline void PutAttDouble(const char * name, double value);
		inline void PutAttString(const char * name, char * value);
		inline void PutAttDateTime(const char * name, time_t value);
		inline void PutAttByte(const char * name, byte value);
		inline void PutAttSignedByte(const char * name, sbyte value);
		inline void PutAttBoolean(const char * name, bool value);
		/*Get*Scalar*/
		inline short GetAttShort(const char * name);
		inline int GetAttInt(const char * name);
		inline long long GetAttLongLong(const char * name);
		inline unsigned short GetAttUnsignedShort(const char * name);
		inline unsigned int GetAttUnsignedInt(const char * name);
		inline unsigned long long GetAttUnsignedLongLong(const char * name);
		inline float GetAttFloat(const char * name);
		inline double GetAttDouble(const char * name);
		inline char * GetAttString(const char * name);
		inline time_t GetAttDateTime(const char * name);
		inline byte GetAttByte(const char * name);
		inline sbyte GetAttSignedByte(const char * name);
		inline bool GetAttBoolean(const char * name);
		/*Put*Array*/
		inline void PutAttShorts(const char * name, short * values, int length);
		inline void PutAttInts(const char * name, int * values, int length);
		inline void PutAttLongLongs(const char * name, long long * values, int length);
		inline void PutAttUnsignedShorts(const char * name, unsigned short * values, int length);
		inline void PutAttUnsignedInts(const char * name, unsigned int * values, int length);
		inline void PutAttUnsignedLongLongs(const char * name, unsigned long long * values, int length);
		inline void PutAttFloats(const char * name, float * values, int length);
		inline void PutAttDoubles(const char * name, double * values, int length);
		inline void PutAttStrings(const char * name, char ** values, int length);
		inline void PutAttDateTimes(const char * name, time_t * values, int length);
		inline void PutAttBytes(const char * name, byte * values , int length);
		inline void PutAttSignedBytes(const char * name, sbyte * values, int length);
		inline void PutAttBooleans(const char * name, bool * values, int length);
		/*Get*Array*/
		inline short * GetAttShorts(const char * name, int * length);
		inline int * GetAttInts(const char * name, int * length);
		inline long long * GetAttLongLongs(const char * name, int * length);
		inline unsigned short * GetAttUnsignedShorts(const char * name, int * length);
		inline unsigned int * GetAttUnsignedInts(const char * name, int * length);
		inline unsigned long long * GetAttUnsignedLongLongs(const char * name, int * length);
		inline float * GetAttFloats(const char * name, int * length);
		inline double * GetAttDoubles(const char * name, int * length);
		inline char ** GetAttStrings(const char * name, int * length);
		inline time_t * GetAttDateTimes(const char * name, int * length);
		inline byte * GetAttBytes(const char * name, int * length);
		inline sbyte * GetAttSignedBytes(const char * name, int * length);
		inline bool * GetAttBooleans(const char * name, int * length);
		/******************Schema methods*****************/
		inline char ** Attributes(int * count);
		inline char ** Dimensions(int * count);
		inline int GetShape(int index);
		DataType GetType()
		{
			return this->_type;
		}
		int GetRank()
		{
			return this->_variable->Rank;
		}
		inline DataType GetAttributeType(const char * name);
		/*************************************************/
	};

	DataType CppVariable::GetAttributeType(const char * name)
	{
		System::String ^ strName = gcnew System::String(name);
		if(!this->_variable->Metadata->ContainsKey(strName))
			throw gcnew System::ArgumentException(System::String::Format(
					"No '{0}' Attribute in the dataset", strName));
		System::Object ^ metaData = this->_variable->Metadata[strName];
		return TypeConverter::TypeToEnum(metaData -> GetType());
	}

	void  CppVariable::_InitType()
	{
		this -> _type = TypeConverter::TypeToEnum(this -> _variable -> TypeOfData);
	}
	template<typename T> void CppVariable::_PutTs(T * Values, int * shape)
	{
		if(this->_variable->TypeOfData!=T::typeid)
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast values to ") + T::typeid -> Name+ " array");
		//get Variable dimensions count
		int dimensionsCount = this -> _variable -> Rank;
		//transform dimension sizes to array
		array<System::Int32>^ dimensionsSizesArr = gcnew array<System::Int32>(dimensionsCount);
		for(int ind = 0; ind < dimensionsSizesArr -> Length; ind++)
		{
			dimensionsSizesArr[ind] = shape[ind];
		}
		//array with multidimensional data to put
		System::Array^ arrayToPut = System::Array::CreateInstance(T::typeid, dimensionsSizesArr);
		//current multidimensional index
		MulDimIndex  index(dimensionsSizesArr);
		//now turn flat array into multidimensional array
		for(int ind = 0; ind < arrayToPut -> LongLength; ind++)
		{
			arrayToPut -> SetValue(Values[ind], index.Current());
			if(index.CanAdd1())
				index.Add1();
		}
		this -> _variable -> PutData(arrayToPut);
	}

	void CppVariable::_PutStrings(char ** Values, int * shape)
	{
		if(this->_variable->TypeOfData!=System::String::typeid)
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast values to String array"));
		//get Variable dimensions count
		int dimensionsCount = this -> _variable -> Rank;
		//transform dimension sizes to array
		array<System::Int32>^ dimensionsSizesArr = gcnew array<System::Int32>(dimensionsCount);
		for(int ind = 0; ind < dimensionsSizesArr -> Length; ind++)
		{
			dimensionsSizesArr[ind] = shape[ind];
		}
		typedef System::String^ T;
		//array with multidimensional data to put
		System::Array^ arrayToPut = System::Array::CreateInstance(T::typeid, dimensionsSizesArr);
		//current multidimensional index
		MulDimIndex  index(dimensionsSizesArr);
		//now turn flat array into multidimensional array
		for(int ind = 0; ind < arrayToPut -> LongLength; ind++)
		{
			System::String ^ stringValue  = gcnew System::String(Values[ind]);
			arrayToPut -> SetValue(stringValue, index.Current());
			if(index.CanAdd1())
				index.Add1();
		}
		this -> _variable -> PutData(arrayToPut);
	}

	void CppVariable::_PutDateTimes(time_t * Values, int * shape)
	{
		if(this->_variable->TypeOfData!=System::DateTime::typeid)
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast values to DateTime array"));
		//get Variable dimensions count
		int dimensionsCount = this -> _variable -> Rank;
		int ValuesLength = 0;
		for(int ind = 0; ind < dimensionsCount; ind++)
		{
			ValuesLength += shape[ind];
		}
		//transform dimension sizes to array
		array<System::Int32>^ dimensionsSizesArr = gcnew array<System::Int32>(dimensionsCount);
		for(int ind = 0; ind < dimensionsSizesArr -> Length; ind++)
		{
			dimensionsSizesArr[ind] = shape[ind];
		}
		typedef System::DateTime^ T;
		//array with multidimensional data to put
		System::Array^ arrayToPut = System::Array::CreateInstance(T::typeid, dimensionsSizesArr);
		array<System::DateTime> ^ inArr = TimeConverter::TimeTarrToDateTimeArr(Values, ValuesLength);
		//current multidimensional index
		MulDimIndex  index(dimensionsSizesArr);
		//now turn flat array into multidimensional array
		for(int ind = 0; ind < arrayToPut -> LongLength; ind++)
		{
			arrayToPut -> SetValue(inArr[ind], index.Current());
			if(index.CanAdd1())
				index.Add1();
		}
		this -> _variable -> PutData(arrayToPut);
	}


	char ** CppVariable::_GetStrings(int ** shape)
	{
		char ** result = NULL;
		System::Array ^ dataArr = this -> _variable -> GetData();
		if(dataArr->GetType()->GetElementType()!=System::String::typeid)
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast data to String array"));
		if(dataArr -> LongLength > 0)
		{
			int rank = dataArr -> Rank;
			if(dataArr -> LongLength>(size_t)-1)
				throw gcnew System::InvalidOperationException("Data length is more than size_t. It should be readen by parts.");
			result = new char *[dataArr ->LongLength];
			//current multidimensional index
			array<System::Int32> ^ shapeArr = gcnew array<System::Int32>(rank);
			//fill lendthArr with dimensions length
			for (size_t ind = 0; ind < shapeArr -> Length; ind++)
			{
				shapeArr[ind] = dataArr -> GetLength(ind);
			}
			MulDimIndex index(shapeArr);
			for(size_t ind=0; ind < dataArr -> LongLength; ind++)
			{
				System::Object^ objValue = dataArr -> GetValue(index.Current());
				System::String^ stringValue = System::Convert::ToString(objValue);
				result[ind] = StringBuffer::CopyManagedString(stringValue);
				if(index.CanAdd1())
					index.Add1();
			}
			*shape = ToLink(shapeArr);
			return result;
		}
		else return result;
	}
	char ** CppVariable::_GetStrings(int * shape, int * origin)
	{
		char ** result = NULL;
		int rank = this -> _variable -> Rank;
		//transform shape and origin into array
		array<int> ^ shapeArr = gcnew array<int>(rank);
		for(int ind = 0; ind < shapeArr -> Length; ind++)
		{
			shapeArr[ind] = shape[ind];
		}
		array<int> ^ originArr = gcnew array<int>(rank);
		for(int ind = 0; ind < originArr -> Length; ind++)
		{
			originArr[ind] = origin[ind];
		}
		System::Array ^ dataArr = this -> _variable -> GetData(originArr, shapeArr);
		if(dataArr->GetType()->GetElementType()!=System::String::typeid)
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast data to String array"));
		if(dataArr -> LongLength > 0)
		{
			if(dataArr -> LongLength>(size_t)-1)
				throw gcnew System::InvalidOperationException("Data length is more than size_t. It should be readen by parts.");
			result = new char *[dataArr ->LongLength];
			//current multidimensional index
			MulDimIndex index(shapeArr);
			for(size_t ind=0; ind < dataArr -> LongLength; ind++)
			{
				System::Object^ objValue = dataArr -> GetValue(index.Current());
				System::String^ stringValue = System::Convert::ToString(objValue);
				result[ind] = StringBuffer::CopyManagedString(stringValue);
				if(index.CanAdd1())
					index.Add1();
			}
			return result;
		}
		else return result;
	}

	time_t * CppVariable::_GetDateTimes(int ** shape)
	{
		time_t * result = NULL;
		System::Array ^ dataArr = this -> _variable -> GetData();
		if(dataArr->GetType()->GetElementType()!=System::DateTime::typeid)
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast data to DateTime array"));
		if(dataArr -> LongLength > 0)
		{
			int rank = dataArr -> Rank;
			if(dataArr -> LongLength>(size_t)-1)
				throw gcnew System::InvalidOperationException("Data length is more than type_t. It should be readen by parts.");
			result = new time_t[dataArr -> LongLength];
			array<System::Int32> ^ shapeArr = gcnew array<System::Int32>(rank);
			//fill lendthArr with dimensions length
			for (int ind = 0; ind < shapeArr -> Length; ind++)
			{
				shapeArr[ind] = dataArr -> GetLength(ind);
			}
			//current multidimensional index
			MulDimIndex index(shapeArr);
			System::DateTime origin = TimeConverter::_Origin();
			System::TimeZone ^ timeZone = System::TimeZone::CurrentTimeZone;
			for(size_t ind = 0; ind < dataArr -> LongLength; ind++)
			{
				System::Object^ objValue = dataArr -> GetValue(index.Current());
				System::DateTime dateTimeValue = System::Convert::ToDateTime(objValue);
				result[ind] = TimeConverter::_DateTimeToTimeT(dateTimeValue, origin, timeZone);
				if(index.CanAdd1())
					index.Add1();
			}
			*shape = ToLink(shapeArr);
			return result;
		}
		else return result;
	}
	time_t * CppVariable::_GetDateTimes(int * shape, int * origin)
	{
		time_t * result = NULL;
		int rank = this -> _variable -> Rank;
		//transform shape and origin into array
		array<int> ^ shapeArr = gcnew array<int>(rank);
		for(int ind = 0; ind < shapeArr -> Length; ind++)
		{
			shapeArr[ind] = shape[ind];
		}
		array<int> ^ originArr = gcnew array<int>(rank);
		for(int ind = 0; ind < originArr -> Length; ind++)
		{
			originArr[ind] = origin[ind];
		}
		System::Array ^ dataArr = this -> _variable -> GetData(originArr, shapeArr);
		if(dataArr->GetType()->GetElementType()!=System::DateTime::typeid)
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast data to DateTime array"));
		if(dataArr -> LongLength > 0)
		{
			if(dataArr -> LongLength>(size_t)-1)
				throw gcnew System::InvalidOperationException("Data length is more than type_t. It should be readen by parts.");
			result = new time_t[dataArr -> LongLength];
			//current multidimensional index
			MulDimIndex index(shapeArr);
			System::DateTime origin = TimeConverter::_Origin();
			System::TimeZone ^ timeZone = System::TimeZone::CurrentTimeZone;
			for(size_t ind = 0; ind < dataArr -> LongLength; ind++)
			{
				System::Object^ objValue = dataArr -> GetValue(index.Current());
				System::DateTime dateTimeValue = System::Convert::ToDateTime(objValue);
				result[ind] = TimeConverter::_DateTimeToTimeT(dateTimeValue, origin, timeZone);
				if(index.CanAdd1())
					index.Add1();
			}
			return result;
		}
		else return result;
	}

	template<typename T> T * CppVariable::_GetTs(int ** shape)
	{
		T * result = NULL;
		System::Array ^ dataArr = this-> _variable -> GetData();
		if(dataArr->GetType()->GetElementType()!=T::typeid)
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast data to ") + T::typeid -> Name+ " array");
		if(dataArr -> LongLength > 0)
		{
			int rank = dataArr -> Rank;
			if(dataArr -> LongLength>(size_t)-1)
				throw gcnew System::InvalidOperationException("Data length is more than size_t. It should be readen by parts.");
			result = new T [dataArr -> LongLength];
			array<System::Int32> ^ shapeArr = gcnew array<System::Int32>(rank);
			//fill lendthArr with dimensions length
			for (int ind = 0; ind < shapeArr -> Length; ind++)
			{
				shapeArr[ind] = dataArr -> GetLength(ind);
			}
			//current multidimensional index
			MulDimIndex index(shapeArr);
			for(size_t ind=0; ind < dataArr -> LongLength; ind++)
			{
				System::Object^ objValue = dataArr -> GetValue(index.Current());
				T tValue = safe_cast<T>(objValue);
				result[ind] = tValue;
				if(index.CanAdd1())
					index.Add1();
			}
			*shape = ToLink(shapeArr);
			return result;
		}
		else return result;
	}
	template<typename T> T * CppVariable::_GetTs(int * shape, int * origin)
	{
		T * result = NULL;
		int rank = this -> _variable -> Rank;
		//transform shape and origin into array
		array<int> ^ shapeArr = gcnew array<int>(rank);
		for(int ind = 0; ind < shapeArr -> Length; ind++)
		{
			shapeArr[ind] = shape[ind];
		}
		array<int> ^ originArr = gcnew array<int>(rank);
		for(int ind = 0; ind < originArr -> Length; ind++)
		{
			originArr[ind] = origin[ind];
		}
		System::Array ^ dataArr = this -> _variable -> GetData(originArr, shapeArr);
		if(dataArr->GetType()->GetElementType()!=T::typeid)
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast data to ") + T::typeid -> Name+ " array");
		if(dataArr -> LongLength > 0)
		{
			if(dataArr -> LongLength>(size_t)-1)
				throw gcnew System::InvalidOperationException("Data length is more than size_t. It should be readen by parts.");
			result = new T [dataArr -> LongLength];
			//current multidimensional index
			MulDimIndex  index(shapeArr);
			for(size_t ind = 0; ind < dataArr -> LongLength; ind++)
			{
				System::Object^ objValue = dataArr -> GetValue(index.Current());
				T tValue = safe_cast<T>(objValue);
				result[ind] = tValue;
				if(index.CanAdd1())
					index.Add1();
			}
			return result;
		}
		else return result;
	}

		template <typename T> T CppVariable::_GetScalarAtt(const char * name)
	{
		System::String ^ stringName = gcnew System::String(name);
		if(!this->_variable->Metadata->ContainsKey(stringName))
			throw gcnew System::ArgumentException(System::String::Format(
					"No '{0}' Attribute in the dataset", stringName));
		System::Object^ metaData =
		this -> _variable -> Metadata[stringName];
		if(metaData->GetType()!=T::typeid)
				throw gcnew System::InvalidCastException(gcnew System::String("Can't cast metadata to ") + T::typeid -> Name);
		return safe_cast<T>(metaData);
	}

	template <typename T> void CppVariable::_PutScalarAtt(const char * name, T value)
	{
		this -> _variable -> Metadata[gcnew System::String(name)] = value;
	}

	template <typename T> T * CppVariable::_GetAtt(const char * name, int * length)
	{
		System::String ^ stringName = gcnew System::String(name);
		if(!this->_variable->Metadata->ContainsKey(stringName))
			throw gcnew System::ArgumentException(System::String::Format(
					"No '{0}' Attribute in the dataset", stringName));
		System::Object^ metaData =
		this -> _variable -> Metadata[stringName];
		if(metaData -> GetType() -> IsArray)
		{
			if(metaData->GetType()->GetElementType()!=T::typeid)
				throw gcnew System::InvalidCastException(gcnew System::String("Can't cast metadata to ") + T::typeid -> Name+ " array");
			System::Array ^ metaDataArray = (System::Array ^)metaData;
			T* result = new T[metaDataArray -> Length];
			for(int ind = 0;ind < metaDataArray -> Length;ind++)
			{
				result[ind] = safe_cast<T>(metaDataArray->GetValue(ind));
			}
			*length = metaDataArray -> Length;
			return result;
		}
		else throw gcnew System::InvalidCastException(gcnew System::String("Can't cast metadata to array"));
	}

	template <typename T> void CppVariable::_PutAtt(const char * name, T * value, int length)
	{
		array<T>^ out = gcnew array<T>(length);
		for(int ind = 0;ind < out -> Length;ind++)
		{
			out[ind] = value[ind];
		}
		this -> _variable -> Metadata[gcnew System::String(name)] = out;
	}

	void CppVariable::PutAttShort(const char * name, short value)
	{
		this -> _PutScalarAtt<short>(name, value);
	}
	void CppVariable::PutAttInt(const char * name, int value)
	{
		this -> _PutScalarAtt<int>(name, value);
	}
	void CppVariable::PutAttLongLong(const char * name, long long value)
	{
		this -> _PutScalarAtt<long long>(name, value);
	}
	void CppVariable::PutAttUnsignedShort(const char * name, unsigned short value)
	{
		this -> _PutScalarAtt<unsigned short>(name, value);
	}
	void CppVariable::PutAttUnsignedInt(const char * name, unsigned int value)
	{
		this -> _PutScalarAtt<unsigned int>(name, value);
	}
	void CppVariable::PutAttUnsignedLongLong(const char * name, unsigned long long value)
	{
		this -> _PutScalarAtt<unsigned long long>(name, value);
	}
	void CppVariable::PutAttFloat(const char * name, float value)
	{
		this -> _PutScalarAtt<float>(name, value);
	}
	void CppVariable::PutAttDouble(const char * name, double value)
	{
		this -> _PutScalarAtt<double>(name, value);
	}
	void CppVariable::PutAttString(const char * name, char * value)
	{
		this -> _variable -> Metadata[gcnew System::String(name)] =
				gcnew System::String(value);
	}
	void CppVariable::PutAttDateTime(const char * name, time_t value)
	{
		System::DateTime dateTime = TimeConverter::TimeTToDateTime(value);
			this -> _variable-> Metadata[gcnew System::String(name)] = dateTime;
	}
	void CppVariable::PutAttByte(const char * name, byte value)
	{
		this -> _PutScalarAtt<byte>(name, value);
	}
	void CppVariable::PutAttSignedByte(const char * name, sbyte value)
	{
		this -> _PutScalarAtt<sbyte>(name, value);
	}
	void CppVariable::PutAttBoolean(const char * name, bool value)
	{
		this -> _PutScalarAtt<bool>(name, value);
	}

	short CppVariable::GetAttShort(const char * name)
	{
		return this->_GetScalarAtt<short>(name);
	}
	int CppVariable::GetAttInt(const char * name)
	{
		return this->_GetScalarAtt<int>(name);
	}
	long long CppVariable::GetAttLongLong(const char * name)
	{
		return this->_GetScalarAtt<long long>(name);
	}
	unsigned short CppVariable::GetAttUnsignedShort(const char * name)
	{
		return this->_GetScalarAtt<unsigned short>(name);
	}
	unsigned int CppVariable::GetAttUnsignedInt(const char * name)
	{
		return this->_GetScalarAtt<unsigned int>(name);
	}
	unsigned long long CppVariable::GetAttUnsignedLongLong(const char * name)
	{
		return this->_GetScalarAtt<unsigned long long>(name);
	}
	float CppVariable::GetAttFloat(const char * name)
	{
		return this->_GetScalarAtt<float>(name);
	}
	double CppVariable::GetAttDouble(const char * name)
	{
		return this->_GetScalarAtt<double>(name);
	}
	char * CppVariable::GetAttString(const char * name)
	{
		System::String ^ stringName = gcnew System::String(name);
		if(!this->_variable->Metadata->ContainsKey(stringName))
			throw gcnew System::ArgumentException(System::String::Format(
					"No '{0}' Attribute in the dataset", stringName));
		System::Object^ metaData =
		this -> _variable -> Metadata[stringName];
		if(metaData->GetType()!=System::String::typeid)
				throw gcnew System::InvalidCastException(gcnew System::String("Can't cast metadata to String"));
		return StringBuffer::CopyManagedString(safe_cast<System::String ^>(metaData));
	}
	time_t CppVariable::GetAttDateTime(const char * name)
	{
		System::String ^ stringName = gcnew System::String(name);
		if(!this->_variable->Metadata->ContainsKey(stringName))
			throw gcnew System::ArgumentException(System::String::Format(
					"No '{0}' Attribute in the dataset", stringName));
		System::Object^ metaData =
		this ->_variable -> Metadata[stringName];
		if(metaData->GetType()!=System::DateTime::typeid)
				throw gcnew System::InvalidCastException(gcnew System::String("Can't cast metadata to DateTime"));
		System::DateTime dateTimeMetaData = safe_cast<System::DateTime>(metaData);
		return TimeConverter::DateTimeToTimeT(dateTimeMetaData);
	}
	byte CppVariable::GetAttByte(const char * name)
	{
		return this->_GetScalarAtt<byte>(name);
	}
	sbyte CppVariable::GetAttSignedByte(const char * name)
	{
		return this -> _GetScalarAtt<sbyte>(name);
	}
	bool CppVariable::GetAttBoolean(const char * name)
	{
		return this -> _GetScalarAtt<bool>(name);
	}

	void CppVariable::PutAttShorts(const char * name, short * values, int length)
	{
		this -> _PutAtt<short>(name, values, length);
	}
	void CppVariable::PutAttInts(const char * name, int * values, int length)
	{
		this -> _PutAtt<int>(name, values, length);
	}
	void CppVariable::PutAttLongLongs(const char * name, long long * values, int length)
	{
		this -> _PutAtt<long long>(name, values, length);
	}
	void CppVariable::PutAttUnsignedShorts(const char * name, unsigned short * values, int length)
	{
		this -> _PutAtt<unsigned short>(name, values, length);
	}
	void CppVariable::PutAttUnsignedInts(const char * name, unsigned int * values, int length)
	{
		this -> _PutAtt<unsigned int>(name, values, length);
	}
	void CppVariable::PutAttUnsignedLongLongs(const char * name, unsigned long long * values, int length)
	{
		this -> _PutAtt<unsigned long long>(name, values, length);
	}
	void CppVariable::PutAttFloats(const char * name, float * values, int length)
	{
		this -> _PutAtt<float>(name, values, length);
	}
	void CppVariable::PutAttDoubles(const char * name, double * values, int length)
	{
		this -> _PutAtt<double>(name, values, length);
	}
	void CppVariable::PutAttStrings(const char * name, char ** values, int length)
	{
		array<System::String ^>^ out = gcnew array<System::String ^>(length);
		for(int ind = 0; ind < out -> Length;ind++)
		{
			out[ind] = gcnew System::String(values[ind]);
		}
		this -> _variable -> Metadata[gcnew System::String(name)] = out;
	}
	void CppVariable::PutAttDateTimes(const char * name, time_t * values, int length)
	{
		this -> _variable -> Metadata[gcnew System::String(name)] =
			TimeConverter::TimeTarrToDateTimeArr(values, length);
	}
	void CppVariable::PutAttBytes(const char * name, byte * values , int length)
	{
		this -> _PutAtt<byte>(name, values, length);
	}
	void CppVariable::PutAttSignedBytes(const char * name, sbyte * values, int length)
	{
		this -> _PutAtt<sbyte>(name, values, length);
	}
	void CppVariable::PutAttBooleans(const char * name, bool * values, int length)
	{
		this -> _PutAtt<bool>(name, values, length);
	}
	short * CppVariable::GetAttShorts(const char * name, int * length)
	{
		return this -> _GetAtt<short>(name, length);
	}
	int * CppVariable::GetAttInts(const char * name, int * length)
	{
		return this -> _GetAtt<int>(name, length);
	}
	long long * CppVariable::GetAttLongLongs(const char * name, int * length)
	{
		return this -> _GetAtt<long long>(name, length);
	}
	unsigned short * CppVariable::GetAttUnsignedShorts(const char * name, int * length)
	{
		return this -> _GetAtt<unsigned short>(name, length);
	}
	unsigned int * CppVariable::GetAttUnsignedInts(const char * name, int * length)
	{
		return this -> _GetAtt<unsigned int>(name, length);
	}
	unsigned long long * CppVariable::GetAttUnsignedLongLongs(const char * name, int * length)
	{
		return this -> _GetAtt<unsigned long long>(name, length);
	}
	float * CppVariable::GetAttFloats(const char * name, int * length)
	{
		return this -> _GetAtt<float>(name, length);
	}
	double * CppVariable::GetAttDoubles(const char * name, int * length)
	{
		return this -> _GetAtt<double>(name, length);
	}
	char ** CppVariable::GetAttStrings(const char * name, int * length)
	{
		System::String ^ stringName = gcnew System::String(name);
		if(!this->_variable->Metadata->ContainsKey(stringName))
			throw gcnew System::ArgumentException(System::String::Format(
					"No '{0}' Attribute in the dataset", stringName));
		System::Object^ metaData =
		this ->_variable -> Metadata[stringName];
		if(metaData -> GetType() -> IsArray)
		{
			if(metaData->GetType()->GetElementType()!=System::String::typeid)
				throw gcnew System::InvalidCastException(gcnew System::String("Can't cast metadata to String array"));
			System::Array ^ out = (System::Array ^)metaData;
			*length = out -> Length;
			char ** ret = new char*[out -> Length];
			for(int ind = 0; ind < out -> Length; ind++)
			{
				ret[ind] = StringBuffer::CopyManagedString(System::Convert::ToString(out->GetValue(ind)));
			}
			return ret;
		}
		else throw gcnew System::InvalidCastException(gcnew System::String("Can't cast metadata to array"));
	}
	time_t * CppVariable::GetAttDateTimes(const char * name, int * length)
	{
		System::String ^ stringName = gcnew System::String(name);
		if(!this->_variable->Metadata->ContainsKey(stringName))
			throw gcnew System::ArgumentException(System::String::Format(
					"No '{0}' Attribute in the dataset", stringName));
		System::Object^ metaData =
		this ->_variable -> Metadata[stringName];
		if(metaData -> GetType() -> IsArray)
		{
			if(metaData->GetType()->GetElementType()!=System::DateTime::typeid)
				throw gcnew System::InvalidCastException(gcnew System::String("Can't cast metadata to DateTime array"));
			System::Array ^ metaDataArray = (System::Array ^)metaData;
			*length = metaDataArray -> Length;
			return TimeConverter::DateTimeArrToTimeTarr(metaDataArray);
		}
		else throw gcnew System::InvalidCastException(gcnew System::String("Can't cast metadata to array"));
	}
	byte * CppVariable::GetAttBytes(const char * name, int * length)
	{
		return this -> _GetAtt<byte>(name, length);
	}
	sbyte * CppVariable::GetAttSignedBytes(const char * name, int * length)
	{
		return this -> _GetAtt<sbyte>(name, length);
	}
	bool * CppVariable::GetAttBooleans(const char * name, int * length)
	{
		return this -> _GetAtt<bool>(name, length);
	}

	char ** CppVariable::Dimensions(int * count)
	{
		array<System::String ^> ^ dimensions = this -> _variable -> Dimensions -> AsNamesArray();
		*count = dimensions -> Length;
		if(*count > 0)
		{
			char ** result = new char*[dimensions -> Length];
			for(int ind = 0; ind < dimensions -> Length; ind++)
			{
				result[ind] = StringBuffer::CopyManagedString(dimensions[ind]);
			}
			return result;
		}
		else return NULL;
	}

	char ** CppVariable::Attributes(int * count)
	{
		*count = this->_variable->Metadata->Count;
		if(*count > 0)
		{
			char** result = new char*[*count];
			int ind = 0;
			for each(System::Collections::Generic::KeyValuePair<System::String^, System::Object^> pair in this->_variable->Metadata)
			{
				result[ind++] = StringBuffer::CopyManagedString(pair.Key);
			}
			return result;
		}
		else return NULL;
	}

	class DataSetRef 
	{
	private:
		gcroot<ScSData::DataSet^> _dataset;
		int _refcount;
		DataSetRef(ScSData::DataSet^ dataset)
		{
			_dataset = dataset;
			_refcount = 1;
		}

		VariableTable _variableTable;
	public:
		static DataSetRef *Create(ScSData::DataSet^ dataset)
		{
			DataSetRef *result = new DataSetRef(dataset);
			if(!result)
				throw std::exception("Out of memory");
			return result;
		}

		void AddRef()
		{
			_refcount++;
		}

		static void Release(DataSetRef *dsr)
		{
			if(--dsr->_refcount <= 0) {
				delete dsr->_dataset; // Also calls Dispose for IDisposable
				delete dsr;
			}
		}

		ScSData::DataSet^ GetDataSet()
		{
			return _dataset;
		}

		void AddVariableRef(CppVariable * variable)
		{
			this->_variableTable.AddVariableRef(variable);
		}
	};

	public class CppDataSet
	{
	private:
		DataSetRef *_dataset;
		template <typename T> inline T _GetScalarAtt(const char * name);
		template <typename T> inline void _PutScalarAtt(const char * name, T value);
		template <typename T> inline T * _GetAtt(const char * name, int * length);
		template <typename T> inline void _PutAtt(const char * name, T * value, int length);
		inline DataType CppDataSet::_TypeToEnum(System::Type ^ type);
		inline CppVariable * _AddVar(DataType type, const char * name, array<System::String ^> ^ dimsArray);
	public:
		CppDataSet(const wchar_t * cstr)
		{
			_dataset = DataSetRef::Create(
				ScSData::DataSet::Open(gcnew System::String(cstr)));
		}

		CppDataSet(const char * cstr)
		{
			_dataset = DataSetRef::Create(
				ScSData::DataSet::Open(gcnew System::String(cstr)));
		}

		CppDataSet(System::String ^ cstr)
		{
			_dataset = DataSetRef::Create(ScSData::DataSet::Open(cstr));
		}

		CppDataSet(const CppDataSet &cds)
		{
			_dataset = cds._dataset;
			_dataset->AddRef();
		}

		~CppDataSet()
		{
			DataSetRef::Release(_dataset);
		}

		static CppDataSet * Open(const char * cstr)
		{
			CppDataSet * ret = new CppDataSet(cstr);
			return ret;
		}

		inline CppVariable * Add(DataType type, const char * name,
			const char * dim);
		inline CppVariable * Add(DataType type, const char * name,
			char ** dims, int dimsNumber);

		inline VariableElement Value(const char * name, int index);
		inline VariableElement Value(const char * name, int index1, int index2);
		inline VariableElement Value(const char * name, int * indexArr);
		//Array get put methods
		/****************GET****************/
		inline char ** GetStrings(const char * name, int ** shape);
		inline char ** GetStrings(const char * name, int * shape, int * origin);
		inline short * GetShorts(const char * name, int ** shape);
		inline short * GetShorts(const char * name, int * shape, int * origin);
		inline int * GetInts(const char * name, int ** shape);
		inline int * GetInts(const char * name, int * shape, int * origin);
		inline long long * GetLongLongs(const char * name, int ** shape);
		inline long long * GetLongLongs(const char * name, int * shape, int * origin);
		inline unsigned short * GetUnsignedShorts(const char * name, int ** shape);
		inline unsigned short * GetUnsignedShorts(const char * name, int * shape, int * origin);
		inline unsigned int * GetUnsignedInts(const char * name, int ** shape);
		inline unsigned int * GetUnsignedInts(const char * name, int * shape, int * origin);
		inline unsigned long long * GetUnsignedLongLongs(const char * name, int ** shape);
		inline unsigned long long * GetUnsignedLongLongs(const char * name, int * shape, int * origin);
		inline float * GetFloats(const char * name, int ** shape);
		inline float * GetFloats(const char * name, int * shape, int * origin);
		inline double * GetDoubles(const char * name, int ** shape);
		inline double * GetDoubles(const char * name, int * shape, int * origin);
		inline time_t * GetDateTimes(const char * name, int ** shape);
		inline time_t * GetDateTimes(const char * name, int * shape, int * origin);
		inline byte * GetBytes(const char * name, int ** shape);
		inline byte * GetBytes(const char * name, int * shape, int * origin);
		inline sbyte * GetSignedBytes(const char * name, int ** shape);
		inline sbyte * GetSignedBytes(const char * name, int * shape, int * origin);
		inline bool * GetBooleans(const char * name, int ** shape);
		inline bool * GetBooleans(const char * name, int * shape, int * origin);
		/****************PUT****************/
		inline void PutShorts(const char * name, short * values, int * shape);
		inline void PutInts(const char * name, int * values, int * shape);
		inline void PutLongLongs(const char * name,long long * values, int * shape);
		inline void PutUnsignedShorts(const char * name, unsigned short * values, int * shape);
		inline void PutUnsignedInts(const char * name, unsigned int * values, int * shape);
		inline void PutUnsignedLongLongs(const char * name, unsigned long long * values, int * shape);
		inline void PutFloats(const char * name, float * values, int * shape);
		inline void PutDoubles(const char * name, double * values, int * shape);
		inline void PutStrings(const char * name, char ** values, int * shape);
		inline void PutDateTimes(const char * name, time_t * values, int * shape);
		inline void PutBytes(const char * name, byte * values, int * shape);
		inline void PutSignedBytes(const char * name, sbyte * values, int * shape);
		inline void PutBooleans(const char * name, bool * values, int * shape);
		//Scalar get set methods
		//Multidimensional index array
		/****************GET****************/
		inline int GetInt(const char * name, int * indexArr);
		inline short GetShort(const char * name, int * indexArr);
		inline long long GetLongLong(const char * name, int * indexArr);
		inline unsigned short GetUnsignedShort(const char * name, int * indexArr);
		inline unsigned int GetUnsignedInt(const char * name, int * indexArr);
		inline unsigned long long GetUnsignedLongLong(const char * name, int * indexArr);
		inline float GetFloat(const char * name, int * indexArr);
		inline double GetDouble(const char * name, int * indexArr);
		inline char * GetString(const char * name, int * indexArr);
		inline time_t GetDateTime(const char * name, int * indexArr);
		inline byte GetByte(const char * name, int * indexArr);
		inline sbyte GetSignedByte(const char * name, int * indexArr);
		inline bool GetBoolean(const char * name, int * indexArr);
		/****************PUT****************/
		inline void PutShort(const char * name, short value, int * indexArr);
		inline void PutInt(const char * name, int value, int * indexArr);
		inline void PutLongLong(const char * name, long long value, int * indexArr);
		inline void PutUnsignedShort(const char * name, unsigned short value, int * indexArr);
		inline void PutUnsignedInt(const char * name, unsigned int value, int * indexArr);
		inline void PutUnsignedLongLong(const char * name, unsigned long long value, int * indexArr);
		inline void PutFloat(const char * name, float value, int * indexArr);
		inline void PutDouble(const char * name, double value, int * indexArr);
		inline void PutString(const char * name, char * value, int * indexArr);
		inline void PutDateTime(const char * name, time_t value, int * indexArr);
		inline void PutByte(const char * name, byte value, int * indexArr);
		inline void PutSignedByte(const char * name, sbyte value, int * indexArr);
		inline void PutBoolean(const char * name, bool value, int * indexArr);
		//Scalar get set methods
		//One dimension index array
		/****************GET****************/
		inline int GetInt(const char * name, int index);
		inline short GetShort(const char * name, int index);
		inline long long GetLongLong(const char * name, int index);
		inline unsigned short GetUnsignedShort(const char * name, int index);
		inline unsigned int GetUnsignedInt(const char * name, int index);
		inline unsigned long long GetUnsignedLongLong(const char * name, int index);
		inline float GetFloat(const char * name, int index);
		inline double GetDouble(const char * name, int index);
		inline char * GetString(const char * name, int index);
		inline time_t GetDateTime(const char * name, int index);
		inline byte GetByte(const char * name, int index);
		inline sbyte GetSignedByte(const char * name, int index);
		inline bool GetBoolean(const char * name, int index);
		/****************PUT****************/
		inline void PutShort(const char * name, short value, int index);
		inline void PutInt(const char * name, int value, int index);
		inline void PutLongLong(const char * name, long long value, int index);
		inline void PutUnsignedShort(const char * name, unsigned short value, int index);
		inline void PutUnsignedInt(const char * name, unsigned int value, int index);
		inline void PutUnsignedLongLong(const char * name, unsigned long long value, int index);
		inline void PutFloat(const char * name, float value, int index);
		inline void PutDouble(const char * name, double value, int index);
		inline void PutString(const char * name, char * value, int index);
		inline void PutDateTime(const char * name, time_t value, int index);
		inline void PutByte(const char * name, byte value, int index);
		inline void PutSignedByte(const char * name, sbyte value, int index);
		inline void PutBoolean(const char * name, bool value, int index);
		//MetaData methods
		/*Put*Scalar*/
		inline void PutAttShort(const char * name, short value);
		inline void PutAttInt(const char * name, int value);
		inline void PutAttLongLong(const char * name, long long value);
		inline void PutAttUnsignedShort(const char * name, unsigned short value);
		inline void PutAttUnsignedInt(const char * name, unsigned int value);
		inline void PutAttUnsignedLongLong(const char * name, unsigned long long value);
		inline void PutAttFloat(const char * name, float value);
		inline void PutAttDouble(const char * name, double value);
		inline void PutAttString(const char * name, char * value);
		inline void PutAttDateTime(const char * name, time_t value);
		inline void PutAttByte(const char * name, byte value);
		inline void PutAttSignedByte(const char * name, sbyte value);
		inline void PutAttBoolean(const char * name, bool value);
		/*Get*Scalar*/
		inline short GetAttShort(const char * name);
		inline int GetAttInt(const char * name);
		inline long long GetAttLongLong(const char * name);
		inline unsigned short GetAttUnsignedShort(const char * name);
		inline unsigned int GetAttUnsignedInt(const char * name);
		inline unsigned long long GetAttUnsignedLongLong(const char * name);
		inline float GetAttFloat(const char * name);
		inline double GetAttDouble(const char * name);
		inline char * GetAttString(const char * name);
		inline time_t GetAttDateTime(const char * name);
		inline byte GetAttByte(const char * name);
		inline sbyte GetAttSignedByte(const char * name);
		inline bool GetAttBoolean(const char * name);
		/*Put*Array*/
		inline void PutAttShorts(const char * name, short * values, int length);
		inline void PutAttInts(const char * name, int * values, int length);
		inline void PutAttLongLongs(const char * name, long long * values, int length);
		inline void PutAttUnsignedShorts(const char * name, unsigned short * values, int length);
		inline void PutAttUnsignedInts(const char * name, unsigned int * values, int length);
		inline void PutAttUnsignedLongLongs(const char * name, unsigned long long * values, int length);
		inline void PutAttFloats(const char * name, float * values, int length);
		inline void PutAttDoubles(const char * name, double * values, int length);
		inline void PutAttStrings(const char * name, char ** values, int length);
		inline void PutAttDateTimes(const char * name, time_t * values, int length);
		inline void PutAttBytes(const char * name, byte * values , int length);
		inline void PutAttSignedBytes(const char * name, sbyte * values, int length);
		inline void PutAttBooleans(const char * name, bool * values, int length);
		/*Get*Array*/
		inline short * GetAttShorts(const char * name, int * length);
		inline int * GetAttInts(const char * name, int * length);
		inline long long * GetAttLongLongs(const char * name, int * length);
		inline unsigned short * GetAttUnsignedShorts(const char * name, int * length);
		inline unsigned int * GetAttUnsignedInts(const char * name, int * length);
		inline unsigned long long * GetAttUnsignedLongLongs(const char * name, int * length);
		inline float * GetAttFloats(const char * name, int * length);
		inline double * GetAttDoubles(const char * name, int * length);
		inline char ** GetAttStrings(const char * name, int * length);
		inline time_t * GetAttDateTimes(const char * name, int * length);
		inline byte * GetAttBytes(const char * name, int * length);
		inline sbyte * GetAttSignedBytes(const char * name, int * length);
		inline bool * GetAttBooleans(const char * name, int * length);
		/*Append simple*/
		inline void AppendShort(const char * name, short value);
		inline void AppendInt(const char * name, int value);
		inline void AppendLongLong(const char * name, long long value);
		inline void AppendUnsignedShort(const char * name, unsigned short value);
		inline void AppendUnsignedInt(const char * name, unsigned int value);
		inline void AppendUnsignedLongLong(const char * name, unsigned long long value);
		inline void AppendFloat(const char * name, float value);
		inline void AppendDouble(const char * name, double value);
		inline void AppendString(const char * name, char * value);
		inline void AppendDateTime(const char * name, time_t value);
		inline void AppendByte(const char * name, byte value);
		inline void AppendSignedByte(const char * name, sbyte value);
		inline void AppendBoolean(const char * name, bool value);
		/*Append dim*/
		inline void AppendShort(const char * name, short value, const char * dim);
		inline void AppendInt(const char * name, int value, const char * dim);
		inline void AppendLongLong(const char * name, long long value, const char * dim);
		inline void AppendUnsignedShort(const char * name, unsigned short value, const char * dim);
		inline void AppendUnsignedInt(const char * name, unsigned int value, const char * dim);
		inline void AppendUnsignedLongLong(const char * name, unsigned long long value, const char * dim);
		inline void AppendFloat(const char * name, float value, const char * dim);
		inline void AppendDouble(const char * name, double value, const char * dim);
		inline void AppendString(const char * name, char * value, const char * dim);
		inline void AppendDateTime(const char * name, time_t value, const char * dim);
		inline void AppendByte(const char * name, byte value, const char * dim);
		inline void AppendSignedByte(const char * name, sbyte value, const char * dim);
		inline void AppendBoolean(const char * name, bool value, const char * dim);

		/*Append*Array*/
		inline void AppendShorts(const char * name, short * values, int valuesRank , int * shape);
		inline void AppendInts (const char * name, int * values, int valuesRank , int * shape);
		inline void AppendLongLongs(const char * name, long long * values, int valuesRank , int * shape);
		inline void AppendUnsignedShorts(const char * name, unsigned short * values, int valuesRank , int * shape);
		inline void AppendUnsignedInts(const char * name, unsigned int * values, int valuesRank , int * shape);
		inline void AppendUnsignedLongLongs(const char * name, unsigned long long * values, int valuesRank , int * shape);
		inline void AppendFloats(const char * name, float * values, int valuesRank , int * shape);
		inline void AppendDoubles(const char * name, double * values, int valuesRank , int * shape);
		inline void AppendStrings(const char * name, char ** values, int valuesRank , int * shape);
		inline void AppendDateTimes(const char * name, time_t * values, int valuesRank , int * shape);
		inline void AppendBytes(const char * name, byte * values, int valuesRank , int * shape);
		inline void AppendSignedBytes(const char * name, sbyte * values, int valuesRank , int * shape);
		inline void AppendBooleans(const char * name, bool * values, int valuesRank , int * shape);

		inline void AppendShorts(const char * name, short * values, int valuesRank , int * shape, const char * dim);
		inline void AppendInts (const char * name, int * values, int valuesRank , int * shape, const char * dim);
		inline void AppendLongLongs(const char * name, long long * values, int valuesRank , int * shape, const char * dim);
		inline void AppendUnsignedShorts(const char * name, unsigned short * values, int valuesRank , int * shape, const char * dim);
		inline void AppendUnsignedInts(const char * name, unsigned int * values, int valuesRank , int * shape, const char * dim);
		inline void AppendUnsignedLongLongs(const char * name, unsigned long long * values, int valuesRank , int * shape, const char * dim);
		inline void AppendFloats(const char * name, float * values, int valuesRank , int * shape, const char * dim);
		inline void AppendDoubles(const char * name, double * values, int valuesRank , int * shape, const char * dim);
		inline void AppendStrings(const char * name, char ** values, int valuesRank , int * shape, const char * dim);
		inline void AppendDateTimes(const char * name, time_t * values, int valuesRank , int * shape, const char * dim);
		inline void AppendBytes(const char * name, byte * values, int valuesRank , int * shape, const char * dim);
		inline void AppendSignedBytes(const char * name, sbyte * values, int valuesRank , int * shape, const char * dim);
		inline void AppendBooleans(const char * name, bool * values, int valuesRank , int * shape, const char * dim);
		/*********************Schema methods***********************/
		inline char ** Dimensions(int * count);
		inline char ** Variables(int * count);
		inline char ** Attributes(int * count);
		inline DataType GetVariableType(const char* name);
		inline DataType GetAttributeType(const char * name);
		inline int GetDimensionLength(const char* name);
		/*********************************************************/

		//CppVariable * Variable(_TCHAR * name);
		CppVariable & Variable(const char * name)
		{
			if (_dataset->GetDataSet()->Variables ->Contains(gcnew System::String(name)))
			{
				CppVariable * variable = new CppVariable(name, _dataset->GetDataSet()->Variables[gcnew System::String(name)]);
				this -> _dataset -> AddVariableRef(variable);
				return *variable;
			}
			else
			{
				throw gcnew System::ArgumentException(System::String::Format(
					"No '{0}' Variable in the dataset", gcnew System::String(name)));
			}
		}

		void Commit()
		{
			GetDataSet()->Commit();
		}

		ScSData::DataSet^ GetDataSet()
		{
			return _dataset->GetDataSet();
		}

		void SetAutoCommitEnabled(bool value)
		{
			GetDataSet()->AutocommitEnabled = value;
		}

		bool Contains(const char name[])
		{
			return GetDataSet()->Variables ->Contains(gcnew System::String(name));
		}
	};

	

	public class VariableElement
	{
	private:
		gcroot<Microsoft::Research::Science::Data::DataSet ^> _dataset;
		gcroot<Microsoft::Research::Science::Data::Variable ^> _variable;
		gcroot<System::String ^> _name;
		gcroot<cli::array<int> ^> _indices;
		int _rank;

	public:
		inline VariableElement(void);
		inline VariableElement(Microsoft::Research::Science::Data::DataSet ^ dataset, const char * name, int index);
		inline VariableElement(Microsoft::Research::Science::Data::DataSet ^ dataset, const char * name, int index1, int index2);
		inline VariableElement(Microsoft::Research::Science::Data::DataSet ^ dataset, const char * name, array<int> ^ indices);
		inline VariableElement(Microsoft::Research::Science::Data::DataSet ^ dataset, const char * name, int * indices);

		inline virtual ~VariableElement();

		inline VariableElement & operator = (System::Single x);
		inline VariableElement & operator = (System::Double x);
		inline VariableElement & operator = (System::Int16 x);
		inline VariableElement & operator = (System::Int32 x);
		inline VariableElement & operator = (System::Int64 x);
		inline VariableElement & operator = (System::UInt16 x);
		inline VariableElement & operator = (System::UInt32 x);
		inline VariableElement & operator = (System::UInt64 x);
		inline VariableElement & operator = (System::Byte x);
		inline VariableElement & operator = (System::SByte x);
		inline VariableElement & operator = (char * x);
		inline VariableElement & operator = (System::Boolean x);

		inline operator System::Single();
		inline operator System::Double();
		inline operator System::Int16();
		inline operator System::Int32();
		inline operator System::Int64();
		inline operator System::UInt16();
		inline operator System::UInt32();
		inline operator System::UInt64();
		inline operator System::Byte();
		inline operator System::SByte();
		inline operator char *();
		inline operator System::Boolean();
	private:
		inline void GetVariable();
	};



	CppVariable::CppVariable( void )
	{

	}

	CppVariable::CppVariable( const char * name, CppDataSet * dataset )
	{
		_name = gcnew System::String(name);
		_dataset = dataset ->GetDataSet();
		if (_dataset ->Variables ->Contains(_name))
		{
			_variable = _dataset ->Variables[_name];
			this->_InitType();
		}
		else
		{
			throw gcnew System::ArgumentException(System::String::Format("Variable {0} doesn't exist in the dataset", _name));
		}

	}

	CppVariable::CppVariable( const char * name, Microsoft::Research::Science::Data::Variable ^ Variable )
	{
		_name = gcnew System::String(name);
		_variable = Variable;
		this->_InitType();
	}

	CppVariable::~CppVariable( void )
	{
		delete _name;
		delete _variable;
		delete _dataset;
	}

	int CppVariable::GetShape( int index )
	{
		if (index < _variable ->Rank)
		{
			return _variable ->GetShape()[index];
		}
		else
		{
			throw gcnew System::ArgumentOutOfRangeException(
				System::String::Format("Variable {0} has rank {1}", _name, _variable ->Rank));
		}
	}

	template<typename T> void CppVariable::_Append( T value )
	{
		if (_variable ->TypeOfData == T::typeid)
		{
			cli::array<T> ^ Arr = gcnew cli::array<T>(1) { value };
			_variable ->Append(Arr);
		}
		else
		{
			throw gcnew System::InvalidCastException(System::String::Format(
				"Can't cast value to Variable type {0}", _variable ->TypeOfData ->ToString()));
		}
	}

	template<typename T> void CppVariable::_Append( T value, const char * dim)
	{
		System::String ^ strDim = gcnew System::String(dim);
		if (_variable ->TypeOfData == T::typeid)
		{
			cli::array<T> ^ Arr = gcnew cli::array<T>(1) { value };
			_variable ->Append(Arr, strDim);
		}
		else
		{
			throw gcnew System::InvalidCastException(System::String::Format(
				"Can't cast value to Variable type {0}", _variable ->TypeOfData ->ToString()));
		}
	}

	template<typename T> void CppVariable::_Append( T * values, int valuesRank , int * shape, const char * dim)
	{
		if (_variable ->TypeOfData == T::typeid)
		{
			System::String ^ strDim = gcnew System::String(dim);
			array<int> ^ dimensionsSizesArr = gcnew array<int>(valuesRank);
			for(int ind = 0; ind < valuesRank; ind++)
			{
				dimensionsSizesArr[ind] = shape[ind];
			}
			System::Array^ Arr = System::Array::CreateInstance(T::typeid, dimensionsSizesArr);
			//current multidimensional index
			MulDimIndex  index(dimensionsSizesArr);
			//now turn flat array into multidimensional array
			for(int ind = 0; ind < Arr -> LongLength; ind++)
			{
				Arr -> SetValue(values[ind], index.Current());
				if(index.CanAdd1())
					index.Add1();
			}
			_variable ->Append(Arr, strDim);
		}
		else
		{
			throw gcnew System::InvalidCastException(System::String::Format(
				"Can't cast values to Variable type {0}", _variable ->TypeOfData ->ToString()));
		}
	}


	template<typename T> void CppVariable::_Append( T * values, int valuesRank , int * shape)
	{
		if (_variable ->TypeOfData == T::typeid)
		{
			array<int> ^ dimensionsSizesArr = gcnew array<int>(valuesRank);
			for(int ind = 0; ind < valuesRank; ind++)
			{
				dimensionsSizesArr[ind] = shape[ind];
			}
			System::Array^ Arr = System::Array::CreateInstance(T::typeid, dimensionsSizesArr);
			//current multidimensional index
			MulDimIndex  index(dimensionsSizesArr);
			//now turn flat array into multidimensional array
			for(int ind = 0; ind < Arr -> LongLength; ind++)
			{
				Arr -> SetValue(values[ind], index.Current());
				if(index.CanAdd1())
					index.Add1();
			}
			_variable ->Append(Arr);
		}
		else
		{
			throw gcnew System::InvalidCastException(System::String::Format(
				"Can't cast values to Variable type {0}", _variable ->TypeOfData ->ToString()));
		}
	}

	
	void CppVariable::_AppendStrings( char ** values, int valuesRank , int * shape, const char * dim)
	{
		if (_variable ->TypeOfData == System::String::typeid)
		{
			System::String ^ strDim = gcnew System::String(dim);
			array<int> ^ dimensionsSizesArr = gcnew array<int>(valuesRank);
			for(int ind = 0; ind < valuesRank; ind++)
			{
				dimensionsSizesArr[ind] = shape[ind];
			}
			System::Array^ Arr = System::Array::CreateInstance(System::String::typeid, dimensionsSizesArr);
			//current multidimensional index
			MulDimIndex  index(dimensionsSizesArr);
			//now turn flat array into multidimensional array
			for(int ind = 0; ind < Arr -> LongLength; ind++)
			{
				System::String ^ stringValue  = gcnew System::String(values[ind]);
				Arr -> SetValue(stringValue, index.Current());
				if(index.CanAdd1())
					index.Add1();
			}
			_variable ->Append(Arr, strDim);
		}
		else
		{
			throw gcnew System::InvalidCastException(System::String::Format(
				"Can't cast values to Variable type {0}", _variable ->TypeOfData ->ToString()));
		}
	}


	void CppVariable::_AppendStrings( char ** values, int valuesRank , int * shape)
	{
		if (_variable ->TypeOfData == System::String::typeid)
		{
			array<int> ^ dimensionsSizesArr = gcnew array<int>(valuesRank);
			for(int ind = 0; ind < valuesRank; ind++)
			{
				dimensionsSizesArr[ind] = shape[ind];
			}
			System::Array^ Arr = System::Array::CreateInstance(System::String::typeid, dimensionsSizesArr);
			//current multidimensional index
			MulDimIndex  index(dimensionsSizesArr);
			//now turn flat array into multidimensional array
			for(int ind = 0; ind < Arr -> LongLength; ind++)
			{
				System::String ^ stringValue  = gcnew System::String(values[ind]);
				Arr -> SetValue(stringValue, index.Current());
				if(index.CanAdd1())
					index.Add1();
			}
			_variable ->Append(Arr);
		}
		else
		{
			throw gcnew System::InvalidCastException(System::String::Format(
				"Can't cast values to Variable type {0}", _variable ->TypeOfData ->ToString()));
		}
	}


	void CppVariable::_AppendDateTimes( time_t * values, int valuesRank , int * shape, const char * dim)
	{
		if (_variable ->TypeOfData == System::DateTime::typeid)
		{
			System::String ^ strDim = gcnew System::String(dim);
			array<int> ^ dimensionsSizesArr = gcnew array<int>(valuesRank);
			for(int ind = 0; ind < valuesRank; ind++)
			{
				dimensionsSizesArr[ind] = shape[ind];
			}
			int ValuesLength = 0;
			for(int ind = 0; ind < valuesRank; ind++)
			{
				ValuesLength += shape[ind];
			}
			array<System::DateTime> ^ inArr = TimeConverter::TimeTarrToDateTimeArr(values, ValuesLength);
			System::Array^ Arr = System::Array::CreateInstance(System::DateTime::typeid, dimensionsSizesArr);
			//current multidimensional index
			MulDimIndex  index(dimensionsSizesArr);
			//now turn flat array into multidimensional array
			for(int ind = 0; ind < Arr -> LongLength; ind++)
			{
				Arr -> SetValue(inArr[ind], index.Current());
				if(index.CanAdd1())
					index.Add1();
			}
			_variable ->Append(Arr, strDim);
		}
		else
		{
			throw gcnew System::InvalidCastException(System::String::Format(
				"Can't cast values to Variable type {0}", _variable ->TypeOfData ->ToString()));
		}
	}


	void CppVariable::_AppendDateTimes( time_t * values, int valuesRank , int * shape)
	{
		if (_variable ->TypeOfData == System::DateTime::typeid)
		{
			array<int> ^ dimensionsSizesArr = gcnew array<int>(valuesRank);
			for(int ind = 0; ind < valuesRank; ind++)
			{
				dimensionsSizesArr[ind] = shape[ind];
			}
			int ValuesLength = 0;
			for(int ind = 0; ind < valuesRank; ind++)
			{
				ValuesLength += shape[ind];
			}
			array<System::DateTime> ^ inArr = TimeConverter::TimeTarrToDateTimeArr(values, ValuesLength);
			System::Array^ Arr = System::Array::CreateInstance(System::DateTime::typeid, dimensionsSizesArr);
			//current multidimensional index
			MulDimIndex  index(dimensionsSizesArr);
			//now turn flat array into multidimensional array
			for(int ind = 0; ind < Arr -> LongLength; ind++)
			{
				Arr -> SetValue(inArr[ind], index.Current());
				if(index.CanAdd1())
					index.Add1();
			}
			_variable ->Append(Arr);
		}
		else
		{
			throw gcnew System::InvalidCastException(System::String::Format(
				"Can't cast values to Variable type {0}", _variable ->TypeOfData ->ToString()));
		}
	}

	void CppVariable::_AppendDateTime( time_t value )
	{
		if (_variable ->TypeOfData == System::DateTime::typeid)
		{
			cli::array<System::DateTime> ^ dateTimeArray = gcnew cli::array<System::DateTime>(1) { TimeConverter::TimeTToDateTime(value) };
			_variable ->Append(dateTimeArray);
		}
		else
		{
			throw gcnew System::InvalidCastException(System::String::Format(
				"Can't cast value to Variable type {0}", _variable ->TypeOfData ->ToString()));
		}
	}

	void CppVariable::_AppendDateTime( time_t value, const char * dim )
	{
		System::String ^ strDim = gcnew System::String(dim);
		if (_variable ->TypeOfData == System::DateTime::typeid)
		{
			cli::array<System::DateTime> ^ Arr = gcnew cli::array<System::DateTime>(1) { TimeConverter::TimeTToDateTime(value) };
			_variable ->Append(Arr, strDim);
		}
		else
		{
			throw gcnew System::InvalidCastException(System::String::Format(
				"Can't cast value to Variable type {0}", _variable ->TypeOfData ->ToString()));
		}
	}

	void CppVariable::_AppendString( char * value )
	{
		if (_variable ->TypeOfData == System::String::typeid)
		{
			cli::array<System::String ^> ^ stringArray = gcnew cli::array<System::String ^>(1) { gcnew System::String(value) };
			_variable ->Append(stringArray);
		}
		else
		{
			throw gcnew System::InvalidCastException(System::String::Format(
				"Can't cast value to Variable type {0}", _variable ->TypeOfData ->ToString()));
		}
	}

	void CppVariable::_AppendString( char * value, const char * dim)
	{
		System::String ^ strDim = gcnew System::String(dim);
		if (_variable ->TypeOfData == System::String::typeid)
		{
			cli::array<System::String ^> ^ stringArray = gcnew cli::array<System::String ^>(1) { gcnew System::String(value) };
			_variable ->Append(stringArray, strDim);
		}
		else
		{
			throw gcnew System::InvalidCastException(System::String::Format(
				"Can't cast value to Variable type {0}", _variable ->TypeOfData ->ToString()));
		}
	}

	CppVariable * CppDataSet::_AddVar(DataType type,const char * name, array<System::String ^> ^ dimsArray)
	{
		if(type == DataType::BOOL)
		{
			return new CppVariable(name, _dataset->GetDataSet()->AddVariable<System::Boolean>(gcnew System::String(name), dimsArray));
		}
		else if(type == DataType::BYTE)
		{
			return new CppVariable(name, _dataset->GetDataSet()->AddVariable<System::Byte>(gcnew System::String(name), dimsArray));
		}
		else if(type == DataType::DATETIME)
		{
			return new CppVariable(name, _dataset->GetDataSet()->AddVariable<System::DateTime>(gcnew System::String(name), dimsArray));
		}
		else if(type == DataType::DOUBLE)
		{
			return new CppVariable(name, _dataset->GetDataSet()->AddVariable<System::Double>(gcnew System::String(name), dimsArray));
		}
		else if(type == DataType::FLOAT)
		{
			return new CppVariable(name, _dataset->GetDataSet()->AddVariable<System::Single>(gcnew System::String(name), dimsArray));
		}
		else if(type == DataType::INT)
		{
			return new CppVariable(name, _dataset->GetDataSet()->AddVariable<System::Int32>(gcnew System::String(name), dimsArray));
		}
		else if(type == DataType::LONGLONG)
		{
			return new CppVariable(name, _dataset->GetDataSet()->AddVariable<System::Int64>(gcnew System::String(name), dimsArray));
		}
		else if(type == DataType::SBYTE)
		{
			return new CppVariable(name, _dataset->GetDataSet()->AddVariable<System::SByte>(gcnew System::String(name), dimsArray));
		}
		else if(type == DataType::SHORT)
		{
			return new CppVariable(name, _dataset->GetDataSet()->AddVariable<System::Int16>(gcnew System::String(name), dimsArray));
		}
		else if(type == DataType::STRING)
		{
			return new CppVariable(name, _dataset->GetDataSet()->AddVariable<System::String^>(gcnew System::String(name), dimsArray));
		}
		else if(type == DataType::UINT)
		{
			return new CppVariable(name, _dataset->GetDataSet()->AddVariable<System::UInt32>(gcnew System::String(name), dimsArray));
		}
		else if(type == DataType::ULONGLONG)
		{
			return new CppVariable(name, _dataset->GetDataSet()->AddVariable<System::UInt64>(gcnew System::String(name), dimsArray));
		}
		else if(type == DataType::USHORT)
		{
			return new CppVariable(name, _dataset->GetDataSet()->AddVariable<System::UInt16>(gcnew System::String(name), dimsArray));
		}
		else throw gcnew System::ArgumentException(gcnew System::String("Variable can't have such type"));
	}

	CppVariable * CppDataSet::Add(DataType type, const char * name, const char * dim)
	{
		array<System::String ^> ^ dimsArray = gcnew array<System::String ^>(1) {gcnew System::String(dim)};
		return this->_AddVar(type, name, dimsArray);
	}

	CppVariable * CppDataSet::Add(DataType type, const char * name, char ** dims, int dimsNumber)
	{
		array<System::String ^> ^ dimsArray = gcnew array<System::String ^>(dimsNumber);
		for (int i = 0; i < dimsNumber; i++)
		{
			dimsArray[i] = gcnew System::String(dims[i]);
		}
		return this->_AddVar(type, name, dimsArray);
	}

	VariableElement CppDataSet::Value(const char * name, int index)
	{
		array<int> ^ indices = gcnew array<int>(1) { index };
		VariableElement variableElement(_dataset->GetDataSet(), name, indices);
		return variableElement;
	}

	VariableElement CppDataSet::Value(const char * name, int index1, int index2)
	{
		array<int> ^ indices = gcnew array<int>(2) {index1, index2};
		VariableElement variableElement(_dataset->GetDataSet(), name, indices);
		return variableElement;
	}

	VariableElement CppDataSet::Value(const char * name, int * indexArr)
	{
		VariableElement variableElement(_dataset->GetDataSet(), name, indexArr);
		return variableElement;
	}
	
	char ** CppDataSet::GetStrings(const char * name, int ** shape)
	{
		CppVariable Variable = this -> Variable(name);
		char ** result = Variable._GetStrings(shape);
		return result;
	}

	char ** CppDataSet::GetStrings(const char * name, int * shape, int * origin)
	{
		CppVariable Variable = this -> Variable(name);
		char ** result = Variable._GetStrings(shape, origin);
		return result;
	}

	short * CppDataSet::GetShorts(const char * name, int ** shape)
	{
		CppVariable Variable = this -> Variable(name);
		short* result = Variable._GetTs<short>(shape);
		return result;
	}

	short * CppDataSet::GetShorts(const char * name, int * shape, int * origin)
	{
		CppVariable Variable = this -> Variable(name);
		short* result = Variable._GetTs<short>(shape, origin);
		return result;
	}

	int * CppDataSet::GetInts(const char * name, int ** shape)
	{
		CppVariable Variable = this -> Variable(name);
		int* result = Variable._GetTs<int>(shape);
		return result;
	}

	int * CppDataSet::GetInts(const char * name, int * shape, int * origin)
	{
		CppVariable Variable = this -> Variable(name);
		int* result = Variable._GetTs<int>(shape, origin);
		return result;
	}

	long long * CppDataSet::GetLongLongs(const char * name, int ** shape)
	{
		CppVariable Variable = this -> Variable(name);
		long long * result = Variable._GetTs<long long>(shape);
		return result;
	}

	long long * CppDataSet::GetLongLongs(const char * name, int * shape, int * origin)
	{
		CppVariable Variable = this -> Variable(name);
		long long * result = Variable._GetTs<long long>(shape, origin);
		return result;
	}

	unsigned short * CppDataSet::GetUnsignedShorts(const char * name, int ** shape)
	{
		CppVariable Variable = this -> Variable(name);
		unsigned short * result = Variable._GetTs<unsigned short>(shape);
		return result;
	}

	unsigned short * CppDataSet::GetUnsignedShorts(const char * name, int * shape, int * origin)
	{
		CppVariable Variable = this -> Variable(name);
		unsigned short * result = Variable._GetTs<unsigned short>(shape, origin);
		return result;
	}

	unsigned int * CppDataSet::GetUnsignedInts(const char * name, int ** shape)
	{
		CppVariable Variable = this -> Variable(name);
		unsigned int * result = Variable._GetTs<unsigned int>(shape);
		return result;
	}

	unsigned int * CppDataSet::GetUnsignedInts(const char * name, int * shape, int * origin)
	{
		CppVariable Variable = this -> Variable(name);
		unsigned int* result = Variable._GetTs<unsigned int>(shape, origin);
		return result;
	}

	unsigned long long * CppDataSet::GetUnsignedLongLongs(const char * name, int ** shape)
	{
		CppVariable Variable = this -> Variable(name);
		unsigned long long * result = Variable._GetTs<unsigned long long>(shape);
		return result;
	}

	unsigned long long * CppDataSet::GetUnsignedLongLongs(const char * name, int * shape, int * origin)
	{
		CppVariable Variable = this -> Variable(name);
		unsigned long long * result = Variable._GetTs<unsigned long long>(shape, origin);
		return result;
	}

    time_t * CppDataSet::GetDateTimes(const char * name, int ** shape)
	{
		CppVariable Variable = this -> Variable(name);
		time_t * result = Variable._GetDateTimes(shape);
		return result;
	}
	time_t * CppDataSet::GetDateTimes(const char * name, int * shape, int * origin)
	{
		CppVariable Variable = this -> Variable(name);
		time_t * result = Variable. _GetDateTimes(shape, origin);
		return result;
	}

	byte * CppDataSet::GetBytes(const char * name, int ** shape)
	{
		CppVariable Variable = this -> Variable(name);
		byte * result = Variable._GetTs<byte>(shape);
		return result;
	}

	byte * CppDataSet::GetBytes(const char * name, int * shape, int * origin)
	{
		CppVariable Variable = this -> Variable(name);
		byte * result = Variable._GetTs<byte>(shape, origin);
		return result;
	}

	sbyte * CppDataSet::GetSignedBytes(const char * name, int ** shape)
	{
		CppVariable Variable = this -> Variable(name);
		sbyte * result = Variable._GetTs<sbyte>(shape);
		return result;
	}

	sbyte * CppDataSet::GetSignedBytes(const char * name, int * shape, int * origin)
	{
		CppVariable Variable = this -> Variable(name);
		sbyte * result = Variable._GetTs<sbyte>(shape, origin);
		return result;
	}

	bool * CppDataSet::GetBooleans(const char * name, int ** shape)
	{
		CppVariable Variable = this -> Variable(name);
		bool * result = Variable._GetTs<bool>(shape);
		return result;
	}

	bool * CppDataSet::GetBooleans(const char * name, int * shape, int * origin)
	{
		CppVariable Variable = this -> Variable(name);
		bool * result = Variable._GetTs<bool>(shape, origin);
		return result;
	}

	double * CppDataSet::GetDoubles(const char * name, int ** shape)
	{
		CppVariable Variable = this -> Variable(name);
		double* result = Variable._GetTs<double>(shape);
		return result;
	}

	float * CppDataSet::GetFloats(const char * name, int ** shape)
	{
		CppVariable Variable = this -> Variable(name);
		float* result = Variable._GetTs<float>(shape);
		return result;
	}

	float * CppDataSet::GetFloats(const char * name, int * shape, int * origin)
	{
		CppVariable Variable = this -> Variable(name);
		float* result = Variable._GetTs<float>(shape, origin);
		return result;
	}

	double * CppDataSet::GetDoubles(const char * name, int * shape, int * origin)
	{
		CppVariable Variable = this -> Variable(name);
		double* result = Variable._GetTs<double>(shape, origin);
		return result;
	}

	void CppDataSet::PutShorts(const char * name, short * Values, int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._PutTs<short>(Values, shape);
	}

	void CppDataSet::PutInts(const char * name, int * Values, int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._PutTs<int>(Values, shape);
	}

	void CppDataSet::PutLongLongs(const char * name, long long * Values, int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._PutTs<long long>(Values, shape);
	}

	void CppDataSet::PutUnsignedShorts(const char * name,unsigned short * Values, int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._PutTs<unsigned short>(Values, shape);
	}


	void CppDataSet::PutUnsignedInts(const char * name,unsigned int * values, int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._PutTs<unsigned int>(values, shape);
	}

	void CppDataSet::PutUnsignedLongLongs(const char * name,unsigned long long * values, int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._PutTs<unsigned long long>(values, shape);
	}

	void CppDataSet::PutFloats(const char * name, float * values, int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._PutTs<float>(values, shape);
	}

	void CppDataSet::PutDoubles(const char * name, double * Values, int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._PutTs<double>(Values, shape);
	}

	void CppDataSet::PutStrings(const char * name, char ** Values, int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._PutStrings(Values, shape);
	}

	void CppDataSet::PutDateTimes(const char * name, time_t * Values, int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._PutDateTimes(Values, shape);
	}

	void CppDataSet::PutBytes(const char * name, byte * values, int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._PutTs<byte>(values, shape);
	}

	void CppDataSet::PutSignedBytes(const char * name, sbyte * values, int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._PutTs<sbyte>(values, shape);
	}

	void CppDataSet::PutBooleans(const char * name, bool * values, int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._PutTs<bool>(values, shape);
	}

	short CppDataSet::GetShort(const char * name, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		return res;
	}

	long long CppDataSet::GetLongLong(const char * name, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		return res;
	}

	int CppDataSet::GetInt(const char * name, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		return res;
	}

	unsigned short CppDataSet::GetUnsignedShort(const char * name, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		return res;
	}
	
	unsigned int CppDataSet::GetUnsignedInt(const char * name, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		return res;
	}

	unsigned long long CppDataSet::GetUnsignedLongLong(const char * name, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		System::UInt64 ret = res;
		return ret;
	}

	void CppDataSet::PutShort(const char * name, short Value, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		res = Value;
	}

	void CppDataSet::PutLongLong(const char * name, long long Value, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		res = Value;
	}

	void CppDataSet::PutInt(const char * name, int Value, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		res = Value;
	}

	void CppDataSet::PutUnsignedShort(const char * name, unsigned short Value, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		res = Value;
	}

	void CppDataSet::PutUnsignedInt(const char * name, unsigned int Value, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		res = Value;
	}

	void CppDataSet::PutUnsignedLongLong(const char * name, unsigned long long Value, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		res = Value;
	}

	float CppDataSet::GetFloat(const char * name, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		return res;
	}

	void CppDataSet::PutFloat(const char * name, float value, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		res = value;
	}

	double CppDataSet::GetDouble(const char * name, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		return res;
	}

	void CppDataSet::PutDouble(const char * name, double Value, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		res = Value;
	}
	
	char * CppDataSet::GetString(const char * name, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		return res;
	}

	void CppDataSet::PutString(const char * name, char * Value, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		res = Value;
	}

	time_t CppDataSet::GetDateTime(const char * name, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		return res;
	}

	void CppDataSet::PutDateTime(const char * name, time_t Value, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		res = Value;
	}

	byte CppDataSet::GetByte(const char * name, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		return res;
	}

	sbyte CppDataSet::GetSignedByte(const char * name, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		return res;
	}

	bool CppDataSet::GetBoolean(const char * name, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		return res;
	}


	void CppDataSet::PutByte(const char * name, byte value, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		res = value;
	}

	void CppDataSet::PutSignedByte(const char * name, sbyte value, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		res = value;
	}

	void CppDataSet::PutBoolean(const char * name, bool value, int * indexArr)
	{
		VariableElement res = this -> Value(name, indexArr);
		res = value;
	}

	short CppDataSet::GetShort(const char * name, int index)
	{
		VariableElement res = this -> Value(name, index);
		return res;
	}

	long long CppDataSet::GetLongLong(const char * name, int index)
	{
		VariableElement res = this -> Value(name, index);
		return res;
	}

	int CppDataSet::GetInt(const char * name, int index)
	{
		VariableElement res = this -> Value(name, index);
		return res;
	}

	unsigned short CppDataSet::GetUnsignedShort(const char * name, int index)
	{
		VariableElement res = this -> Value(name, index);
		return res;
	}
	
	unsigned int CppDataSet::GetUnsignedInt(const char * name, int index)
	{
		VariableElement res = this -> Value(name, index);
		return res;
	}

	unsigned long long CppDataSet::GetUnsignedLongLong(const char * name, int index)
	{
		VariableElement res = this -> Value(name, index);
		System::UInt64 ret = res;
		return ret;
	}

	void CppDataSet::PutShort(const char * name, short Value, int index)
	{
		VariableElement res = this -> Value(name, index);
		res = Value;
	}

	void CppDataSet::PutLongLong(const char * name, long long Value, int index)
	{
		VariableElement res = this -> Value(name, index);
		res = Value;
	}

	void CppDataSet::PutInt(const char * name, int Value, int index)
	{
		VariableElement res = this -> Value(name, index);
		res = Value;
	}

	void CppDataSet::PutUnsignedShort(const char * name, unsigned short Value, int index)
	{
		VariableElement res = this -> Value(name, index);
		res = Value;
	}

	void CppDataSet::PutUnsignedInt(const char * name, unsigned int Value, int index)
	{
		VariableElement res = this -> Value(name, index);
		res = Value;
	}

	void CppDataSet::PutUnsignedLongLong(const char * name, unsigned long long Value, int index)
	{
		VariableElement res = this -> Value(name, index);
		res = Value;
	}

	float CppDataSet::GetFloat(const char * name, int index)
	{
		VariableElement res = this -> Value(name, index);
		return res;
	}

	void CppDataSet::PutFloat(const char * name, float value, int index)
	{
		VariableElement res = this -> Value(name, index);
		res = value;
	}

	double CppDataSet::GetDouble(const char * name, int index)
	{
		VariableElement res = this -> Value(name, index);
		return res;
	}

	void CppDataSet::PutDouble(const char * name, double Value, int index)
	{
		VariableElement res = this -> Value(name, index);
		res = Value;
	}
	
	char * CppDataSet::GetString(const char * name, int index)
	{
		VariableElement res = this -> Value(name, index);
		return res;
	}

	void CppDataSet::PutString(const char * name, char * Value, int index)
	{
		VariableElement res = this -> Value(name, index);
		res = Value;
	}

	time_t CppDataSet::GetDateTime(const char * name, int index)
	{
		VariableElement res = this -> Value(name, index);
		return res;
	}

	void CppDataSet::PutDateTime(const char * name, time_t Value, int index)
	{
		VariableElement res = this -> Value(name, index);
		res = Value;
	}

	byte CppDataSet::GetByte(const char * name, int index)
	{
		VariableElement res = this -> Value(name, index);
		return res;
	}

	sbyte CppDataSet::GetSignedByte(const char * name, int index)
	{
		VariableElement res = this -> Value(name, index);
		return res;
	}

	bool CppDataSet::GetBoolean(const char * name, int index)
	{
		VariableElement res = this -> Value(name, index);
		return res;
	}


	void CppDataSet::PutByte(const char * name, byte value, int index)
	{
		VariableElement res = this -> Value(name, index);
		res = value;
	}

	void CppDataSet::PutSignedByte(const char * name, sbyte value, int index)
	{
		VariableElement res = this -> Value(name, index);
		res = value;
	}

	void CppDataSet::PutBoolean(const char * name, bool value, int index)
	{
		VariableElement res = this -> Value(name, index);
		res = value;
	}

	template <typename T> T CppDataSet::_GetScalarAtt(const char * name)
	{
		System::String ^ stringName = gcnew System::String(name);
		if(!this->_dataset->GetDataSet()->Metadata->ContainsKey(stringName))
			throw gcnew System::ArgumentException(System::String::Format(
					"No '{0}' Attribute in the dataset", stringName));
		System::Object^ metaData =
		this -> _dataset -> GetDataSet() -> Metadata[stringName];
		if(metaData->GetType()!=T::typeid)
				throw gcnew System::InvalidCastException(gcnew System::String("Can't cast metadata to ") + T::typeid -> Name);
		return safe_cast<T>(metaData);
	}

	template <typename T> void CppDataSet::_PutScalarAtt(const char * name, T value)
	{
		this -> _dataset -> GetDataSet() -> Metadata[gcnew System::String(name)] = value;
	}

	template <typename T> T * CppDataSet::_GetAtt(const char * name, int * length)
	{
		System::String ^ stringName = gcnew System::String(name);
		if(!this->_dataset->GetDataSet()->Metadata->ContainsKey(stringName))
			throw gcnew System::ArgumentException(System::String::Format(
					"No '{0}' Attribute in the dataset", stringName));
		System::Object^ metaData =
		this -> _dataset -> GetDataSet() -> Metadata[stringName];
		if(metaData -> GetType() -> IsArray)
		{
			if(metaData->GetType()->GetElementType()!=T::typeid)
				throw gcnew System::InvalidCastException(gcnew System::String("Can't cast metadata to ") + T::typeid -> Name + " array");
			System::Array ^ metaDataArray = (System::Array ^)metaData;
			T* result = new T[metaDataArray -> Length];
			for(int ind = 0;ind < metaDataArray -> Length;ind++)
			{
				result[ind] = safe_cast<T>(metaDataArray->GetValue(ind));
			}
			*length = metaDataArray -> Length;
			return result;
		}
		else throw gcnew System::InvalidCastException(gcnew System::String("Can't cast metadata to array"));
	}

	template <typename T> void CppDataSet::_PutAtt(const char * name, T * value, int length)
	{
		array<T>^ out = gcnew array<T>(length);
		for(int ind = 0;ind < out -> Length;ind++)
		{
			out[ind] = value[ind];
		}
		this -> _dataset -> GetDataSet() -> Metadata[gcnew System::String(name)] = out;
	}

	void CppDataSet::PutAttShort(const char * name, short value)
	{
		this -> _PutScalarAtt<short>(name, value);
	}
	void CppDataSet::PutAttInt(const char * name, int value)
	{
		this -> _PutScalarAtt<int>(name, value);
	}
	void CppDataSet::PutAttLongLong(const char * name, long long value)
	{
		this -> _PutScalarAtt<long long>(name, value);
	}
	void CppDataSet::PutAttUnsignedShort(const char * name, unsigned short value)
	{
		this -> _PutScalarAtt<unsigned short>(name, value);
	}
	void CppDataSet::PutAttUnsignedInt(const char * name, unsigned int value)
	{
		this -> _PutScalarAtt<unsigned int>(name, value);
	}
	void CppDataSet::PutAttUnsignedLongLong(const char * name, unsigned long long value)
	{
		this -> _PutScalarAtt<unsigned long long>(name, value);
	}
	void CppDataSet::PutAttFloat(const char * name, float value)
	{
		this -> _PutScalarAtt<float>(name, value);
	}
	void CppDataSet::PutAttDouble(const char * name, double value)
	{
		this -> _PutScalarAtt<double>(name, value);
	}
	void CppDataSet::PutAttString(const char * name, char * value)
	{
		this -> _dataset -> GetDataSet() -> Metadata[gcnew System::String(name)] =
				gcnew System::String(value);
	}
	void CppDataSet::PutAttDateTime(const char * name, time_t value)
	{
		System::DateTime dateTime = TimeConverter::TimeTToDateTime(value);
			this -> _dataset -> GetDataSet()-> Metadata[gcnew System::String(name)] = dateTime;
	}
	void CppDataSet::PutAttByte(const char * name, byte value)
	{
		this -> _PutScalarAtt<byte>(name, value);
	}
	void CppDataSet::PutAttSignedByte(const char * name, sbyte value)
	{
		this -> _PutScalarAtt<sbyte>(name, value);
	}

	void CppDataSet::PutAttBoolean(const char * name, bool value)
	{
		this -> _PutScalarAtt<bool>(name, value);
	}

	short CppDataSet::GetAttShort(const char * name)
	{
		return this->_GetScalarAtt<short>(name);
	}
	int CppDataSet::GetAttInt(const char * name)
	{
		return this->_GetScalarAtt<int>(name);
	}
	long long CppDataSet::GetAttLongLong(const char * name)
	{
		return this->_GetScalarAtt<long long>(name);
	}
	unsigned short CppDataSet::GetAttUnsignedShort(const char * name)
	{
		return this->_GetScalarAtt<unsigned short>(name);
	}
	unsigned int CppDataSet::GetAttUnsignedInt(const char * name)
	{
		return this->_GetScalarAtt<unsigned int>(name);
	}
	unsigned long long CppDataSet::GetAttUnsignedLongLong(const char * name)
	{
		return this->_GetScalarAtt<unsigned long long>(name);
	}
	float CppDataSet::GetAttFloat(const char * name)
	{
		return this->_GetScalarAtt<float>(name);
	}
	double CppDataSet::GetAttDouble(const char * name)
	{
		return this->_GetScalarAtt<double>(name);
	}
	char * CppDataSet::GetAttString(const char * name)
	{
		System::String ^ stringName = gcnew System::String(name);
		if(!this->_dataset->GetDataSet()->Metadata->ContainsKey(stringName))
			throw gcnew System::ArgumentException(System::String::Format(
					"No '{0}' Attribute in the dataset", stringName));
		System::Object^ metaData =
		this -> _dataset -> GetDataSet() -> Metadata[stringName];
		if(metaData->GetType()!=System::String::typeid)
				throw gcnew System::InvalidCastException(gcnew System::String("Can't cast metadata to String"));
		return StringBuffer::CopyManagedString(safe_cast<System::String ^>(metaData));
	}
	time_t CppDataSet::GetAttDateTime(const char * name)
	{
		System::String ^ stringName = gcnew System::String(name);
		if(!this->_dataset->GetDataSet()->Metadata->ContainsKey(stringName))
			throw gcnew System::ArgumentException(System::String::Format(
					"No '{0}' Attribute in the dataset", stringName));
		System::Object^ metaData =
		this -> _dataset -> GetDataSet() -> Metadata[stringName];
		if(metaData->GetType()!=System::DateTime::typeid)
				throw gcnew System::InvalidCastException(gcnew System::String("Can't cast metadata to DateTime"));
		System::DateTime dateTimeMetaData = safe_cast<System::DateTime>(metaData);
		return TimeConverter::DateTimeToTimeT(dateTimeMetaData);
	}
	byte CppDataSet::GetAttByte(const char * name)
	{
		return this->_GetScalarAtt<byte>(name);
	}
	sbyte CppDataSet::GetAttSignedByte(const char * name)
	{
		return this -> _GetScalarAtt<sbyte>(name);
	}

	bool CppDataSet::GetAttBoolean(const char * name)
	{
		return this -> _GetScalarAtt<bool>(name);
	}

	void CppDataSet::PutAttShorts(const char * name, short * values, int length)
	{
		this -> _PutAtt<short>(name, values, length);
	}
	void CppDataSet::PutAttInts(const char * name, int * values, int length)
	{
		this -> _PutAtt<int>(name, values, length);
	}
	void CppDataSet::PutAttLongLongs(const char * name, long long * values, int length)
	{
		this -> _PutAtt<long long>(name, values, length);
	}
	void CppDataSet::PutAttUnsignedShorts(const char * name, unsigned short * values, int length)
	{
		this -> _PutAtt<unsigned short>(name, values, length);
	}
	void CppDataSet::PutAttUnsignedInts(const char * name, unsigned int * values, int length)
	{
		this -> _PutAtt<unsigned int>(name, values, length);
	}
	void CppDataSet::PutAttUnsignedLongLongs(const char * name, unsigned long long * values, int length)
	{
		this -> _PutAtt<unsigned long long>(name, values, length);
	}
	void CppDataSet::PutAttFloats(const char * name, float * values, int length)
	{
		this -> _PutAtt<float>(name, values, length);
	}
	void CppDataSet::PutAttDoubles(const char * name, double * values, int length)
	{
		this -> _PutAtt<double>(name, values, length);
	}
	void CppDataSet::PutAttStrings(const char * name, char ** values, int length)
	{
		array<System::String ^>^ out = gcnew array<System::String ^>(length);
		for(int ind = 0; ind < out -> Length;ind++)
		{
			out[ind] = gcnew System::String(values[ind]);
		}
		this -> _dataset -> GetDataSet() -> Metadata[gcnew System::String(name)] = out;
	}
	void CppDataSet::PutAttDateTimes(const char * name, time_t * values, int length)
	{
		this -> _dataset -> GetDataSet() -> Metadata[gcnew System::String(name)] =
			TimeConverter::TimeTarrToDateTimeArr(values, length);
	}
	void CppDataSet::PutAttBytes(const char * name, byte * values , int length)
	{
		this -> _PutAtt<byte>(name, values, length);
	}
	void CppDataSet::PutAttSignedBytes(const char * name, sbyte * values, int length)
	{
		this -> _PutAtt<sbyte>(name, values, length);
	}
	void CppDataSet::PutAttBooleans(const char * name, bool * values, int length)
	{
		this -> _PutAtt<bool>(name, values, length);
	}
	short * CppDataSet::GetAttShorts(const char * name, int * length)
	{
		return this -> _GetAtt<short>(name, length);
	}
	int * CppDataSet::GetAttInts(const char * name, int * length)
	{
		return this -> _GetAtt<int>(name, length);
	}
	long long * CppDataSet::GetAttLongLongs(const char * name, int * length)
	{
		return this -> _GetAtt<long long>(name, length);
	}
	unsigned short * CppDataSet::GetAttUnsignedShorts(const char * name, int * length)
	{
		return this -> _GetAtt<unsigned short>(name, length);
	}
	unsigned int * CppDataSet::GetAttUnsignedInts(const char * name, int * length)
	{
		return this -> _GetAtt<unsigned int>(name, length);
	}
	unsigned long long * CppDataSet::GetAttUnsignedLongLongs(const char * name, int * length)
	{
		return this -> _GetAtt<unsigned long long>(name, length);
	}
	float * CppDataSet::GetAttFloats(const char * name, int * length)
	{
		return this -> _GetAtt<float>(name, length);
	}
	double * CppDataSet::GetAttDoubles(const char * name, int * length)
	{
		return this -> _GetAtt<double>(name, length);
	}
	char ** CppDataSet::GetAttStrings(const char * name, int * length)
	{
		System::String ^ stringName = gcnew System::String(name);
		if(!this->_dataset->GetDataSet()->Metadata->ContainsKey(stringName))
			throw gcnew System::ArgumentException(System::String::Format(
					"No '{0}' Attribute in the dataset", stringName));
		System::Object^ metaData =
		this -> _dataset -> GetDataSet() -> Metadata[stringName];
		if(metaData -> GetType() -> IsArray)
		{
			if(metaData->GetType()->GetElementType()!=System::String::typeid)
				throw gcnew System::InvalidCastException(gcnew System::String("Can't cast metadata to String array"));
			System::Array ^ out = (System::Array ^)metaData;
			*length = out -> Length;
			char ** ret = new char*[out -> Length];
			for(int ind = 0; ind < out -> Length; ind++)
			{
				ret[ind] = StringBuffer::CopyManagedString(System::Convert::ToString(out->GetValue(ind)));
			}
			return ret;
		}
		else throw gcnew System::InvalidCastException(gcnew System::String("Can't cast metadata to array"));
	}
	time_t * CppDataSet::GetAttDateTimes(const char * name, int * length)
	{	
		System::String ^ stringName = gcnew System::String(name);
		if(!this->_dataset->GetDataSet()->Metadata->ContainsKey(stringName))
			throw gcnew System::ArgumentException(System::String::Format(
					"No '{0}' Attribute in the dataset", stringName));
		System::Object^ metaData =
		this -> _dataset -> GetDataSet() -> Metadata[stringName];
		if(metaData -> GetType() -> IsArray)
		{
			if(metaData->GetType()->GetElementType()!=System::DateTime::typeid)
				throw gcnew System::InvalidCastException(gcnew System::String("Can't cast metadata to DateTime array"));
			System::Array ^ metaDataArray = (System::Array ^)metaData;
			*length = metaDataArray -> Length;
			return TimeConverter::DateTimeArrToTimeTarr(metaDataArray);
		}
		else throw gcnew System::InvalidCastException(gcnew System::String("Can't cast metadata to array"));
	}
	byte * CppDataSet::GetAttBytes(const char * name, int * length)
	{
		return this -> _GetAtt<byte>(name, length);
	}
	sbyte * CppDataSet::GetAttSignedBytes(const char * name, int * length)
	{
		return this -> _GetAtt<sbyte>(name, length);
	}
	bool * CppDataSet::GetAttBooleans(const char * name, int * length)
	{
		return this -> _GetAtt<bool>(name, length);
	}

	void CppDataSet::AppendShort(const char * name, short value)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<short>(value);
	}
	void CppDataSet::AppendInt(const char * name, int value)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<int>(value);
	}
	void CppDataSet::AppendLongLong(const char * name, long long value)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<long long>(value);
	}
	void CppDataSet::AppendUnsignedShort(const char * name, unsigned short value)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<unsigned short>(value);
	}
	void CppDataSet::AppendUnsignedInt(const char * name, unsigned int value)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<unsigned int>(value);
	}
	void CppDataSet::AppendUnsignedLongLong(const char * name, unsigned long long value)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<unsigned long long>(value);
	}
	void CppDataSet::AppendFloat(const char * name, float value)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<float>(value);
	}
	void CppDataSet::AppendDouble(const char * name, double value)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<double>(value);
	}
	void CppDataSet::AppendString(const char * name, char * value)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._AppendString(value);
	}
	void CppDataSet::AppendDateTime(const char * name, time_t value)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._AppendDateTime(value);
	}
	void CppDataSet::AppendByte(const char * name, byte value)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<byte>(value);
	}
	void CppDataSet::AppendSignedByte(const char * name, sbyte value)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<sbyte>(value);
	}
	void CppDataSet::AppendBoolean(const char * name, bool value)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<bool>(value);
	}

	void CppDataSet::AppendShort(const char * name, short value, const char * dim)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<short>(value, dim);
	}
	void CppDataSet::AppendInt(const char * name, int value, const char * dim)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<int>(value, dim);
	}
	void CppDataSet::AppendLongLong(const char * name, long long value, const char * dim)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<long long>(value, dim);
	}
	void CppDataSet::AppendUnsignedShort(const char * name, unsigned short value, const char * dim)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<unsigned short>(value, dim);
	}
	void CppDataSet::AppendUnsignedInt(const char * name, unsigned int value, const char * dim)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<unsigned int>(value, dim);
	}
	void CppDataSet::AppendUnsignedLongLong(const char * name, unsigned long long value, const char * dim)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<unsigned long long>(value, dim);
	}
	void CppDataSet::AppendFloat(const char * name, float value, const char * dim)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<float>(value, dim);
	}
	void CppDataSet::AppendDouble(const char * name, double value, const char * dim)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<double>(value, dim);
	}
	void CppDataSet::AppendString(const char * name, char * value, const char * dim)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._AppendString(value, dim);
	}
	void CppDataSet::AppendDateTime(const char * name, time_t value, const char * dim)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._AppendDateTime(value, dim);
	}
	void CppDataSet::AppendByte(const char * name, byte value, const char * dim)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<byte>(value, dim);
	}
	void CppDataSet::AppendSignedByte(const char * name, sbyte value, const char * dim)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<sbyte>(value, dim);
	}
	void CppDataSet::AppendBoolean(const char * name, bool value, const char * dim)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<bool>(value, dim);
	}

	void CppDataSet::AppendShorts(const char * name, short * values, int valuesRank , int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<short>(values, valuesRank, shape);
	}
	void CppDataSet::AppendInts(const char * name, int * values, int valuesRank , int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<int>(values, valuesRank, shape);
	}
	void CppDataSet::AppendLongLongs(const char * name, long long * values, int valuesRank , int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<long long>(values, valuesRank, shape);
	}
	void CppDataSet::AppendUnsignedShorts(const char * name, unsigned short * values, int valuesRank , int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<unsigned short>(values, valuesRank, shape);
	}
	void CppDataSet::AppendUnsignedInts(const char * name, unsigned int * values, int valuesRank , int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<unsigned int>(values, valuesRank, shape);
	}
	void CppDataSet::AppendUnsignedLongLongs(const char * name, unsigned long long * values, int valuesRank , int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<unsigned long long>(values, valuesRank, shape);
	}
	void CppDataSet::AppendFloats(const char * name, float * values, int valuesRank , int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<float>(values, valuesRank, shape);
	}
	void CppDataSet::AppendDoubles(const char * name, double * values, int valuesRank , int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<double>(values, valuesRank, shape);
	}
	void CppDataSet::AppendStrings(const char * name, char ** values, int valuesRank , int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._AppendStrings(values, valuesRank, shape);
	}
	void CppDataSet::AppendDateTimes(const char * name, time_t * values, int valuesRank , int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._AppendDateTimes(values, valuesRank, shape);
	}
	void CppDataSet::AppendBytes(const char * name, byte * values, int valuesRank , int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<byte>(values, valuesRank, shape);
	}
	void CppDataSet::AppendSignedBytes(const char * name, sbyte * values, int valuesRank , int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<sbyte>(values, valuesRank, shape);
	}
	void CppDataSet::AppendBooleans(const char * name, bool * values, int valuesRank , int * shape)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<bool>(values, valuesRank, shape);
	}

	
void CppDataSet::AppendShorts(const char * name, short * values, int valuesRank , int * shape, const char * dims)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<short>(values, valuesRank, shape, dims);
	}
	void CppDataSet::AppendInts(const char * name, int * values, int valuesRank , int * shape, const char * dims)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<int>(values, valuesRank, shape, dims);
	}
	void CppDataSet::AppendLongLongs(const char * name, long long * values, int valuesRank , int * shape, const char * dims)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<long long>(values, valuesRank, shape, dims);
	}
	void CppDataSet::AppendUnsignedShorts(const char * name, unsigned short * values, int valuesRank , int * shape, const char * dims)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<unsigned short>(values, valuesRank, shape, dims);
	}
	void CppDataSet::AppendUnsignedInts(const char * name, unsigned int * values, int valuesRank , int * shape, const char * dims)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<unsigned int>(values, valuesRank, shape, dims);
	}
	void CppDataSet::AppendUnsignedLongLongs(const char * name, unsigned long long * values, int valuesRank , int * shape, const char * dims)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<unsigned long long>(values, valuesRank, shape, dims);
	}
	void CppDataSet::AppendFloats(const char * name, float * values, int valuesRank , int * shape, const char * dims)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<float>(values, valuesRank, shape, dims);
	}
	void CppDataSet::AppendDoubles(const char * name, double * values, int valuesRank , int * shape, const char * dims)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<double>(values, valuesRank, shape, dims);
	}
	void CppDataSet::AppendStrings(const char * name, char ** values, int valuesRank , int * shape, const char * dims)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._AppendStrings(values, valuesRank, shape, dims);
	}
	void CppDataSet::AppendDateTimes(const char * name, time_t * values, int valuesRank , int * shape, const char * dims)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._AppendDateTimes(values, valuesRank, shape, dims);
	}
	void CppDataSet::AppendBytes(const char * name, byte * values, int valuesRank , int * shape, const char * dims)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<byte>(values, valuesRank, shape, dims);
	}
	void CppDataSet::AppendSignedBytes(const char * name, sbyte * values, int valuesRank , int * shape, const char * dims)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<sbyte>(values, valuesRank, shape, dims);
	}
	void CppDataSet::AppendBooleans(const char * name, bool * values, int valuesRank , int * shape, const char * dims)
	{
		CppVariable Variable = this -> Variable(name);
		Variable._Append<bool>(values, valuesRank, shape, dims);
	}

	char ** CppDataSet::Dimensions(int * count)
	{
		array<System::String ^> ^ dimensions = this->_dataset->GetDataSet()->Dimensions->AsNamesArray();
		*count = dimensions -> Length;
		if(*count > 0)
		{
			char ** result = new char*[dimensions -> Length];
			for(int ind = 0; ind < dimensions -> Length; ind++)
			{
				result[ind] = StringBuffer::CopyManagedString(dimensions[ind]);
			}
			return result;
		}
		else return NULL;
	}
	char ** CppDataSet::Variables(int * count)
	{
		array<Microsoft::Research::Science::Data::Variable ^> ^ variables =  this->_dataset->GetDataSet()->Variables->ToArray();
		*count = variables -> Length;
		if(*count > 0)
		{
			char ** result = new char*[variables -> Length];
			for(int ind = 0; ind < variables -> Length; ind++)
			{
				result[ind] = StringBuffer::CopyManagedString(variables[ind] -> Name);
			}
			return result;
		} 
		else return NULL;
	}
	char ** CppDataSet::Attributes(int * count)
	{
	    *count = this->_dataset->GetDataSet()->Metadata->Count;
		if(*count > 0)
		{
			char** result = new char*[*count];
			int ind = 0;
			for each(System::Collections::Generic::KeyValuePair<System::String^, System::Object^> pair in this->_dataset->GetDataSet()->Metadata)
			{
				result[ind++] = StringBuffer::CopyManagedString(pair.Key);
			}
			return result;
		}
		else return NULL;
	}

	DataType CppDataSet::GetVariableType(const char * name)
	{
		CppVariable Variable = this -> Variable(name);
		return Variable.GetType();
	}
	DataType CppDataSet::GetAttributeType(const char * name)
	{
		System::String ^ strName = gcnew System::String(name);
		if(!this->_dataset->GetDataSet()->Metadata->ContainsKey(strName))
			throw gcnew System::ArgumentException(System::String::Format(
					"No '{0}' Attribute in the dataset", strName));
		System::Object ^ metaData = this->_dataset->GetDataSet()->Metadata[strName];
		return TypeConverter::TypeToEnum(metaData -> GetType());
	}

	int CppDataSet::GetDimensionLength(const char * name)
	{
		System::String ^ strName = gcnew System::String(name);
		if(!this->_dataset->GetDataSet()->Dimensions->Contains(strName))
			throw gcnew System::ArgumentException(System::String::Format(
					"No '{0}' Dimension in the dataset", strName));
		return this->_dataset->GetDataSet()->Dimensions[strName].Length;
	}

	VariableElement::VariableElement(void)
	{

	}

	VariableElement::VariableElement(ScSData::DataSet ^ dataset, const char * name, int index) :
	_dataset(dataset), _name(gcnew System::String(name))
	{
		_indices = gcnew array<int>(1) { index };
		GetVariable();
	}

	VariableElement::VariableElement(ScSData::DataSet ^ dataset, const char * name, int index1, int index2 ) :
	_dataset(dataset), _name(gcnew System::String(name))
	{
		_indices = gcnew array<int>(2) { index1, index2 };
		GetVariable();
	}

	VariableElement::VariableElement(ScSData::DataSet ^ dataset, const char * name, array<int> ^ indices) :
	_dataset(dataset), _name(gcnew System::String(name)), _indices(indices)
	{
		GetVariable();
	}


	VariableElement::VariableElement(ScSData::DataSet ^ dataset, const char * name, int * indices) :
	_dataset(dataset), _name(gcnew System::String(name))
	{
		if (_dataset ->Variables->Contains(_name))
		{
			_variable = _dataset ->Variables[_name];
			_rank = _variable -> Rank;
		}
		else
		{
			throw gcnew System::ArgumentException(System::String::Format(gcnew System::String("Variable {0} not found"), _name));
		}
		_indices = gcnew array<int>(this -> _rank);
		for(int ind = 0; ind < _indices -> Length;ind++)
		{
			_indices[ind] = indices[ind];
		}
	}



	VariableElement::~VariableElement()
	{
		delete _variable;
		delete _name;
	}

	void VariableElement::GetVariable()
	{
		if (_dataset ->Variables->Contains(_name))
		{
			_variable = _dataset ->Variables[_name];
			if ((_rank = _variable ->Rank) != _indices->GetLength(0))
			{
				throw gcnew System::ArgumentException(gcnew System::String("Rank of the Variable and indices array do not match"));
			}
		}
		else
		{
			throw gcnew System::ArgumentException(System::String::Format(gcnew System::String("Variable {0} not found"), _name));
		}
	}


	// TODO: multidimension
	VariableElement & VariableElement::operator = (System::Single x)
	{
		if (_variable ->TypeOfData == System::Single::typeid)
		{
			cli::array<System::Single> ^ Values = gcnew cli::array<System::Single>(1) { x };
			_variable ->PutData(_indices, Values);
		}
		else
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast float to VariableElement"));
		}

		return *this;
	}


	// TODO: multidimension
	VariableElement & VariableElement::operator = (System::Double x)
	{
		if (_variable ->TypeOfData == System::Double::typeid)
		{
			cli::array<System::Double> ^ Values = gcnew cli::array<System::Double>(1) { x };
			_variable ->PutData(_indices , Values);	
		}
		else
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast double to VariableElement"));
		}


		return *this;
	}

		
	VariableElement & VariableElement::operator = (System::Int16 x)
	{
		if (_variable ->TypeOfData == System::Int16::typeid)
		{
			cli::array<System::Int16> ^ Values = gcnew cli::array<System::Int16>(1) { x };
			_variable ->PutData(_indices, Values);
		}
		else
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast short to VariableElement"));
		}

		return *this;
	}

	
	VariableElement & VariableElement::operator = (System::Int32 x)
	{
		if (_variable ->TypeOfData == System::Int32::typeid)
		{
			cli::array<System::Int32> ^ Values = gcnew cli::array<System::Int32>(1) { x };
			_variable ->PutData(_indices, Values);
		}
		else if (_variable ->TypeOfData == System::DateTime::typeid)
		{
#ifdef _USE_32BIT_TIME_T
	   			cli::array<System::DateTime> ^ Values = gcnew cli::array<System::DateTime>(1) { TimeConverter::TimeTToDateTime(x)};
			_variable ->PutData(_indices, Values);
#else
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast int to VariableElement"));
#endif 
		}
		else
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast int to VariableElement"));
		}

		return *this;
	}

		
	VariableElement & VariableElement::operator = (System::Int64 x)
	{
		if (_variable ->TypeOfData == System::Int64::typeid)
		{
			cli::array<System::Int64> ^ Values = gcnew cli::array<System::Int64>(1) { x };
			_variable ->PutData(_indices, Values);
		}
		else if (_variable ->TypeOfData == System::DateTime::typeid)
		{
#ifndef _USE_32BIT_TIME_T
	   			cli::array<System::DateTime> ^ Values = gcnew cli::array<System::DateTime>(1) { TimeConverter::TimeTToDateTime(x)};
			_variable ->PutData(_indices, Values);
#else
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast int to VariableElement"));
#endif 
		}
		else
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast long long to VariableElement"));
		}

		return *this;
	}


			
	VariableElement & VariableElement::operator = (System::UInt16 x)
	{
		if (_variable ->TypeOfData == System::UInt16::typeid)
		{
			cli::array<System::UInt16> ^ Values = gcnew cli::array<System::UInt16>(1) { x };
			_variable ->PutData(_indices, Values);
		}
		else
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast long long to VariableElement"));
		}

		return *this;
	}

			
	VariableElement & VariableElement::operator = (System::UInt32 x)
	{
		if (_variable ->TypeOfData == System::UInt32::typeid)
		{
			cli::array<System::UInt32> ^ Values = gcnew cli::array<System::UInt32>(1) { x };
			_variable ->PutData(_indices, Values);
		}
		else
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast long long to VariableElement"));
		}

		return *this;
	}

	VariableElement & VariableElement::operator = (System::UInt64 x)
	{
		if (_variable ->TypeOfData == System::UInt64::typeid)
		{
			cli::array<System::UInt64> ^ Values = gcnew cli::array<System::UInt64>(1) { x };
			_variable ->PutData(_indices, Values);
		}
		else
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast long long to VariableElement"));
		}

		return *this;
	}


		
	VariableElement & VariableElement::operator = (System::Byte x)
	{
		if (_variable ->TypeOfData == System::Byte::typeid)
		{
			cli::array<System::Byte> ^ Values = gcnew cli::array<System::Byte>(1) { x };
			_variable ->PutData(_indices, Values);
		}
		else
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast byte to VariableElement"));
		}

		return *this;
	}

	VariableElement & VariableElement::operator = (System::SByte x)
	{
		if (_variable ->TypeOfData == System::SByte::typeid)
		{
			cli::array<System::SByte> ^ Values = gcnew cli::array<System::SByte>(1) { x };
			_variable ->PutData(_indices, Values);
		}
		else
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast sbyte to VariableElement"));
		}

		return *this;
	}


	// TODO: multidimension
	VariableElement & VariableElement::operator = (char * x)
	{
		if (_variable ->TypeOfData == System::String::typeid)
		{
			System::String^ Value = gcnew System::String(x);
			cli::array<System::String ^> ^ Values = gcnew cli::array<System::String ^>(1) { Value };
			_variable ->PutData(_indices, Values);
		}
		else
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast char * to VariableElement"));
		}
		return *this;
	}

	// TODO: multidimension
	VariableElement & VariableElement::operator = (System::Boolean x)
	{
		if (_variable ->TypeOfData == System::Boolean::typeid)
		{
			cli::array<System::Boolean> ^ Values = gcnew cli::array<System::Boolean>(1) { x };
			_variable ->PutData(_indices, Values);
		}
		else
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast boolean to VariableElement"));
		}

		return *this;
	}

		
	VariableElement::operator System::Single()
	{
		if (_variable ->TypeOfData != System::Single::typeid)
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast VariableElement to float"));
		}
		array<System::Int32> ^ shapeArray = gcnew array<System::Int32>(_rank);
		for (int i = 0; i < _rank; i++)
		{
			shapeArray[i] = 1;
		}
		System::Array ^ ValueArray = _variable ->GetData(_indices, shapeArray);
		for (int i = 0; i < _rank; i++)
		{
			shapeArray[i] = 0;
		}
		return System::Convert::ToSingle(ValueArray ->GetValue(shapeArray));
	}

	
	VariableElement::operator System::Double()
	{
		if (_variable ->TypeOfData != System::Double::typeid)
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast VariableElement to double"));
		}
		array<System::Int32> ^ shapeArray = gcnew array<System::Int32>(_rank);
		for (int i = 0; i < _rank; i++)
		{
			shapeArray[i] = 1;
		}
		System::Array ^ ValueArray = _variable ->GetData(_indices, shapeArray);
		for (int i = 0; i < _rank; i++)
		{
			shapeArray[i] = 0;
		}
		return System::Convert::ToDouble(ValueArray ->GetValue(shapeArray));
	}

	VariableElement::operator System::Int16()
	{
		if (_variable ->TypeOfData != System::Int16::typeid)
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast VariableElement to short"));
		}
		array<System::Int32> ^ shapeArray = gcnew array<System::Int32>(_rank);
		for (int i = 0; i < _rank; i++)
		{
			shapeArray[i] = 1;
		}
		System::Array ^ ValueArray = _variable ->GetData(_indices, shapeArray);
		for (int i = 0; i < _rank; i++)
		{
			shapeArray[i] = 0;
		}
		return System::Convert::ToInt16(ValueArray ->GetValue(shapeArray));
	}

	
	VariableElement::operator System::Int32()
	{
		if(_variable ->TypeOfData == System::DateTime::typeid)
		{
			array<System::Int32> ^ shapeArray = gcnew array<System::Int32>(_rank);
			for (int i = 0; i < _rank; i++)
			{
				shapeArray[i] = 1;
			}
			System::Array ^ ValueArray = _variable ->GetData(_indices, shapeArray);
			for (int i = 0; i < _rank; i++)
			{
				shapeArray[i] = 0;
			}
			System::DateTime tempTime = System::Convert::ToDateTime(ValueArray ->GetValue(shapeArray));
			return TimeConverter::DateTimeToTimeT(tempTime);
		}
		else if(_variable ->TypeOfData == System::Int32::typeid)
		{
			array<System::Int32> ^ shapeArray = gcnew array<System::Int32>(_rank);
			for (int i = 0; i < _rank; i++)
			{
				shapeArray[i] = 1;
			}
			System::Array ^ ValueArray = _variable ->GetData(_indices, shapeArray);
			for (int i = 0; i < _rank; i++)
			{
				shapeArray[i] = 0;
			}
			return System::Convert::ToInt32(ValueArray ->GetValue(shapeArray));
		}
		else
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast VariableElement to int"));
		}
	}

	
	VariableElement::operator System::Int64()
	{
		if(_variable ->TypeOfData == System::DateTime::typeid)
		{
			array<System::Int32> ^ shapeArray = gcnew array<System::Int32>(_rank);
			for (int i = 0; i < _rank; i++)
			{
				shapeArray[i] = 1;
			}
			System::Array ^ ValueArray = _variable ->GetData(_indices, shapeArray);
			for (int i = 0; i < _rank; i++)
			{
				shapeArray[i] = 0;
			}
			System::DateTime tempTime = System::Convert::ToDateTime(ValueArray ->GetValue(shapeArray));
			return TimeConverter::DateTimeToTimeT(tempTime);
		}
		else if (_variable ->TypeOfData == System::Int64::typeid)
		{
			array<System::Int32> ^ shapeArray = gcnew array<System::Int32>(_rank);
			for (int i = 0; i < _rank; i++)
			{
				shapeArray[i] = 1;
			}
			System::Array ^ ValueArray = _variable ->GetData(_indices, shapeArray);
			for (int i = 0; i < _rank; i++)
			{
				shapeArray[i] = 0;
			}
			return System::Convert::ToInt64(ValueArray ->GetValue(shapeArray));
		}
		else
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast VariableElement to long long"));
		}
	}

		
	VariableElement::operator System::UInt16()
	{
		if (_variable ->TypeOfData != System::UInt16::typeid)
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast VariableElement to unsigned short"));
		}
		array<System::Int32> ^ shapeArray = gcnew array<System::Int32>(_rank);
		for (int i = 0; i < _rank; i++)
		{
			shapeArray[i] = 1;
		}
		System::Array ^ ValueArray = _variable ->GetData(_indices, shapeArray);
		for (int i = 0; i < _rank; i++)
		{
			shapeArray[i] = 0;
		}
		return System::Convert::ToUInt16(ValueArray ->GetValue(shapeArray));
	}

	
	VariableElement::operator System::UInt32()
	{
		if (_variable ->TypeOfData != System::UInt32::typeid)
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast VariableElement to unsigned int"));
		}
		array<System::Int32> ^ shapeArray = gcnew array<System::Int32>(_rank);
		for (int i = 0; i < _rank; i++)
		{
			shapeArray[i] = 1;
		}
		System::Array ^ ValueArray = _variable ->GetData(_indices, shapeArray);
		for (int i = 0; i < _rank; i++)
		{
			shapeArray[i] = 0;
		}
		return System::Convert::ToUInt32(ValueArray ->GetValue(shapeArray));
	}

	
	VariableElement::operator System::UInt64()
	{
		if (_variable ->TypeOfData != System::UInt64::typeid)
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast VariableElement to unsigned long long"));
		}
		array<System::Int32> ^ shapeArray = gcnew array<System::Int32>(_rank);
		for (int i = 0; i < _rank; i++)
		{
			shapeArray[i] = 1;
		}
		System::Array ^ ValueArray = _variable ->GetData(_indices, shapeArray);
		for (int i = 0; i < _rank; i++)
		{
			shapeArray[i] = 0;
		}
		return System::Convert::ToUInt64(ValueArray ->GetValue(shapeArray));
	}

	// TODO: perhaps memory leak
	// Here was writing after array bounds
	// But it is solved.
	VariableElement::operator char *()
	{
		if (_variable ->TypeOfData != System::String::typeid)
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast VariableElement to char *"));
		}
		array<System::Int32> ^ shapeArray = gcnew array<System::Int32>(_rank);
		for (int i = 0; i < _rank; i++)
		{
			shapeArray[i] = 1;
		}
		System::Array ^ ValueArray = _variable ->GetData(_indices, shapeArray);
		for (int i = 0; i < _rank; i++)
		{
			shapeArray[i] = 0;
		}
		System::String ^ result = System::Convert::ToString(ValueArray ->GetValue(shapeArray));
		marshal_context ^ context = gcnew marshal_context();
		char * char_result = new char[result ->Length + 1];
		memcpy(char_result, context ->marshal_as<const char *>(result), result ->Length + 1);

		delete context;
		return char_result;
	}

	
	VariableElement::operator System::Byte()
	{
		if (_variable ->TypeOfData != System::Byte::typeid)
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast VariableElement to byte"));
		}
		array<System::Int32> ^ shapeArray = gcnew array<System::Int32>(_rank);
		for (int i = 0; i < _rank; i++)
		{
			shapeArray[i] = 1;
		}
		System::Array ^ ValueArray = _variable ->GetData(_indices, shapeArray);
		for (int i = 0; i < _rank; i++)
		{
			shapeArray[i] = 0;
		}
		return System::Convert::ToByte(ValueArray ->GetValue(shapeArray));
	}

	VariableElement::operator System::SByte()
	{
		if (_variable ->TypeOfData != System::SByte::typeid)
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast VariableElement to sbyte"));
		}
		array<System::Int32> ^ shapeArray = gcnew array<System::Int32>(_rank);
		for (int i = 0; i < _rank; i++)
		{
			shapeArray[i] = 1;
		}
		System::Array ^ ValueArray = _variable ->GetData(_indices, shapeArray);
		for (int i = 0; i < _rank; i++)
		{
			shapeArray[i] = 0;
		}
		return System::Convert::ToSByte(ValueArray ->GetValue(shapeArray));
	}

	VariableElement::operator System::Boolean()
	{
		if (_variable ->TypeOfData != System::Boolean::typeid)
		{
			throw gcnew System::InvalidCastException(gcnew System::String("Can't cast VariableElement to bool"));
		}
		array<System::Int32> ^ shapeArray = gcnew array<System::Int32>(_rank);
		for (int i = 0; i < _rank; i++)
		{
			shapeArray[i] = 1;
		}
		System::Array ^ ValueArray = _variable ->GetData(_indices, shapeArray);
		for (int i = 0; i < _rank; i++)
		{
			shapeArray[i] = 0;
		}
		return System::Convert::ToBoolean(ValueArray ->GetValue(shapeArray));
	}
}

