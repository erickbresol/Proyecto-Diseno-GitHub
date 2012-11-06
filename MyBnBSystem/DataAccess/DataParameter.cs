#region Copyright 2012 
//
// Original Author: Erick Brenes Solano
// Logical date of creation: 05/11/2012
#endregion

using System;
using System.Data;

namespace DataAccess
{
	/// <summary>
	/// Represents a stored procedure parameter
	/// </summary>
	public class DataParameter
	{
		#region Constructor
		
		/// <summary>
		/// Default constructor for Parameter
		/// </summary>
		/// <param name="parameterName">Name for parameter</param>
		/// <param name="parameterType">DataType for this parameter</param>
		/// <param name="direction">In, Out, InOut</param>
		/// <param name="value">Value for the parameter</param>
		public DataParameter(string parameterName, DbType parameterType, ParameterDirection direction, object value)
		{
			_Name = parameterName;
			_DbType = parameterType;
			_Direction = direction;
			_Value = value;
            _Size = 0;
		}

        /// <summary>
        /// Default constructor for Parameter
        /// </summary>
        /// <param name="parameterName">Name for parameter</param>
        /// <param name="parameterType">DataType for this parameter</param>
        /// <param name="direction">In, Out, InOut</param>
        /// <param name="value">Value for the parameter</param>
        /// <param name="pSize">Size for this parameter usually require for some kind of output parameters</param>
        public DataParameter(string parameterName, DbType parameterType, ParameterDirection direction, object value, int pSize):this(parameterName, parameterType,direction,value)
        {
            _Size = pSize;
        }

		#endregion
		#region Properties

		/// <summary>
		/// Paramenter name
		/// </summary>
		public string ParameterName
		{
			get
			{
				return _Name;
			}
		}

		/// <summary>
		/// Data type for this parameter
		/// </summary>
		public DbType DbType
		{
			get
			{
				return _DbType;
			}
		}

		/// <summary>
		/// Parameter value
		/// </summary>
		public object Value
		{
			get
			{
				return _Value;
			}
			set
			{
				_Value = value;
			}
		}

		/// <summary>
		/// Defines the parameter direction IN, OUT, INOUT, RETURN_VALUE
		/// </summary>
		public ParameterDirection Direction
		{
			get
			{
				return _Direction;
			}
		}

        /// <summary>
        /// Size of the parameter
        /// </summary>
        public int Size
        {
            get
            {
                return _Size;
            }
        }
		#endregion
		#region Attributes

		/// <summary>
		/// DB Type for this parameter
		/// </summary>
		private DbType _DbType;

		/// <summary>
		/// Direction IN, OUT, RETURN_VALUE
		/// </summary>
		private ParameterDirection _Direction;

		/// <summary>
		/// Parameter name
		/// </summary>
		private string _Name;

		/// <summary>
		/// Value of the parameter
		/// </summary>
		private Object _Value;
        /// <summary>
        /// Size of the parameter usually is required for some kind of output parameters
        /// </summary>
        private int _Size;
		#endregion
	}
}