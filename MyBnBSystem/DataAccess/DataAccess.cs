#region Copyright 2012
//
// This software is the proprietary information of MyBnB Corp.
// Use is subject to license terms.
//
// Original Author: Erick Brenes
// Logical date of creation: 05/11/2012
#endregion

using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Xml;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Logging;
using System.Transactions;

namespace DataAccess
{
	/// <summary>
	/// Super class that enabled database access
	/// </summary>
	public class DataAccess
	{
		#region Constructors

		public DataAccess()
		{
            ConnectionStringName = DEFAULT_DATABASE_NAME;
        }
		/// <summary>
		/// Constructor to specify the ConnectionString to use
		/// </summary>
		/// <param name="pConnectionString">Name of the connection string to use</param>
		public DataAccess(string pConnectionString): this()
		{
			ConnectionStringName = pConnectionString;
		}
		#endregion
		#region Properties
		/// <summary>
		/// Stores the name of the connection string to use
		/// </summary>
		public string ConnectionStringName
		{
			get;
			set;
		}

        public DbConnection CurrentConnection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets any error meesage
        /// </summary>
        public string LastErrorMessage
        {
            get;
            set;
        }

		#endregion
		#region Methods
        #region Configuration and General use
        /// <summary>
		/// Get the Database object used from the enterprise library application block
		/// </summary>
		/// <returns>Database object that allows database access</returns>
        protected virtual Database GetDatabase()
		{
            try
            {                
                if (_GeneralDatabase == null)
                {
                    _GeneralDatabase = DatabaseFactory.CreateDatabase(ConnectionStringName);
                }
                return _GeneralDatabase;
            }
            catch (Exception ex)
            {
                LastErrorMessage = ex.Message;
                return null;
            }
		}


		/// <summary>
		/// Prepare a DbCommand object with its parameters
		/// </summary>
		/// <param name="pStoredProcedureName">Stored procedure name using dbo.Name sintax</param>
		/// <param name="pParameters">Array of DataParameter</param>
		/// <param name="pOutputParameters">List of DataParameters that are for output purpose</param>
        /// <param name="pDbAccess">Database Object</param>
        /// <returns>Prepared DbCommand</returns>
        protected virtual DbCommand PrepareCommand(string pStoredProcedureName, IEnumerable<DataParameter> pParameters, out IList<DataParameter> pOutputParameters, out Database pDbAccess)
		{
			pDbAccess = GetDatabase();
			DbCommand command = pDbAccess.GetStoredProcCommand(pStoredProcedureName);

			
			pOutputParameters = new List<DataParameter>();
			pOutputParameters.Add(new DataParameter(RETURN_VALUE, DbType.Int32, ParameterDirection.ReturnValue, 0));

			MySqlParameter param = new MySqlParameter(RETURN_VALUE, MySqlDbType.Int);
			param.Direction = ParameterDirection.ReturnValue;
			command.Parameters.Add(param);

			foreach (DataParameter parameter in pParameters)
			{
				pDbAccess.AddParameter(command, parameter.ParameterName, parameter.DbType, parameter.Direction, parameter.ParameterName, DataRowVersion.Current, parameter.Value);
				if (parameter.Direction==ParameterDirection.Output || parameter.Direction==ParameterDirection.InputOutput) 
				{
					pOutputParameters.Add(parameter);
				}
			}
			return command;
		}

		/// <summary>
		/// Fill the parameters value when they are for output
		/// </summary>
		/// <param name="pOutputParameters">Output parameter list</param>
		/// <param name="pCommand">DBCommand with the parameters array full filled</param>
        protected virtual void FillOutputParameters(ref IList<DataParameter> pOutputParameters, DbCommand pCommand)
		{
			foreach(DataParameter parameter in pOutputParameters)
			{
				if ((string.Compare(parameter.ParameterName, RETURN_VALUE) == 0) && (pCommand.Parameters[parameter.ParameterName].Value == null))
				{
					parameter.Value = 0;
				}
				else
				{
					parameter.Value = pCommand.Parameters[parameter.ParameterName].Value;
				}
			}
		}
		#endregion 
		#region Execute Data Set

		/// <summary>
		/// Extract one DataSet using all the parameters required
		/// </summary>
		/// <param name="storedProcedureName">Stored procedure name using dbo.Name sintax</param>
		/// <param name="parameters">Array of DataParameter</param>
		/// <returns>Full fill dataset</returns>
		protected DataSet ExecuteDataSet(string pStoredProcedureName, DataParameter[] pParameters)
		{
			int returnValue;
			return ExecuteDataSet(pStoredProcedureName, pParameters, out returnValue);
		}

		/// <summary>
		/// Extract one DataSet using all the parameters required
		/// </summary>
		/// <param name="storedProcedureName">Stored procedure name using dbo.Name sintax</param>
		/// <param name="parameters">Array of DataParameter</param>
		/// <param name="returnValue">Returns stored procedure return code</param>
		/// <returns>Full fill dataset</returns>
		protected virtual DataSet ExecuteDataSet(string pStoredProcedureName, DataParameter[] pParameters, out int pReturnValue)
		{
			DbCommand command = null;
			pReturnValue = NULL_VALUE;
			try
			{
				IList<DataParameter> outputParameters;
				Database dbAccess;
				command = PrepareCommand(pStoredProcedureName, pParameters,
				                         out outputParameters, out dbAccess);
				DataSet result = dbAccess.ExecuteDataSet(command);
				FillOutputParameters(ref outputParameters, command);
				pReturnValue = (int)outputParameters[RETURN_VALUE_PARAM_INDEX].Value;

				return result;
			}
			catch(Exception ex)
			{
                LastErrorMessage = ex.Message;
				return null;
			}
			finally
			{
                if (command != null && command.Connection != null && command.Connection.State == ConnectionState.Open && Transaction.Current==null)
				{
					command.Connection.Close();
				}
			}
		}

		#endregion Execute Data Set
		#region Execute Scalar

		/// <summary>
		/// Execute a nonquery stored procedure and get its return value
		/// </summary>
		/// <param name="storedProcedureName">Stored procedure name using dbo.Name sintax</param>
		/// <param name="parameters">Array of DataParameter</param>
		/// <returns>Stored procedure return code</returns>
		protected virtual int ExecuteScalar(string pStoredProcedureName, DataParameter[] pParameters)
		{
			DbCommand command = null;
			try
			{
				IList<DataParameter> outputParameters;
				Database dbAccess;
				command = PrepareCommand(pStoredProcedureName, pParameters, out outputParameters, out dbAccess);

				dbAccess.ExecuteNonQuery(command);
				FillOutputParameters(ref outputParameters, command);

				return (int)outputParameters[RETURN_VALUE_PARAM_INDEX].Value;
			}
			catch(Exception ex)
			{
                LastErrorMessage = ex.Message;
				return NULL_VALUE;
			}
			finally
			{
                if (command != null && command.Connection != null && command.Connection.State == ConnectionState.Open && Transaction.Current == null)
				{
					command.Connection.Close();
				}
			}
		}

		#endregion Execute Scalar
		#endregion
		#region Constants
        /// <summary>
        /// Constant for the name of return value in MySQL Server
        /// </summary>
        private string RETURN_VALUE = "RETURN_VALUE";
        /// <summary>
        /// Default value for a returning null value
        /// </summary>
        protected int NULL_VALUE = -1;
        /// <summary>
        /// Index for the param that must have the return value 
        /// </summary>
        private int RETURN_VALUE_PARAM_INDEX = 0;       
        /// <summary>
        /// Default name for the main database access connection string
        /// </summary>
        public static readonly string DEFAULT_DATABASE_NAME = "mybnbConn";
        #endregion
        #region Attributes
        /// <summary>
        /// Object which stored the general database manager provide by the application block
        /// </summary>
        private Database _GeneralDatabase;
        #endregion
    }
}