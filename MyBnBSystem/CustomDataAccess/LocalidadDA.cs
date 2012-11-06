using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess;
using CommonLibrary;
using System.Data;

namespace CustomDataAccess
{
    public class LocalidadDA : DataAccess.DataAccess
    {
        #region Properties
        public static LocalidadDA Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (_LockObject)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new LocalidadDA();
                        }
                    }
                }
                return _Instance;
            }
        }
        #endregion
        #region Methods

        public List<Localidad> getLocalidades(double pLimitAmount)
        {
            List<Localidad> result = new List<Localidad>();

            try
            {
                DataSet resultDS = ExecuteDataSet("dbo.SP_ObtenerLocalidades", new DataParameter[] 
                {
                    new DataParameter("MontoLimite", System.Data.DbType.Decimal, System.Data.ParameterDirection.Input, pLimitAmount)
                });

                if ((resultDS != null) && (resultDS.Tables.Count > 0) && (resultDS.Tables[0].Rows.Count > 0))
                {
                    foreach (DataRow record in resultDS.Tables[0].Rows)
                    {
                        Localidad _newLocalidad = new Localidad();
                        _newLocalidad.idLocalidad = Convert.ToInt32(record["IdLocalidad"].ToString());
                        _newLocalidad.Pais = Convert.ToString(record["Pais"].ToString());
                        _newLocalidad.Estado = Convert.ToString(record["Estado"].ToString());
                        _newLocalidad.Ciudad = Convert.ToString(record["Ciudad"].ToString());

                        result.Add(_newLocalidad);
                    }
                }                
            }
            catch (Exception ex)
            {                
                LastErrorMessage = ex.Message;
            }
            return result;
        }
        public void insertLocalidad(Localidad pLocalidad)
        {
            ExecuteScalar("dbo.SP_InsertarLocalidad", new DataParameter[]
            {
                new DataParameter("pPais",DbType.String,ParameterDirection.Input,pLocalidad.Pais),
                new DataParameter("pEstado",DbType.String,ParameterDirection.Input,pLocalidad.Estado),
                new DataParameter("pCiudad",DbType.String,ParameterDirection.Input,pLocalidad.Ciudad),
            });
        }

        #endregion
        #region Attributes
        
        private static Object _LockObject = new Object();
        private static LocalidadDA _Instance;

        #endregion
    }
}
