using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibrary;
using CustomDataAccess;

namespace AppClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var _localidadDA = new LocalidadDA();

            var _newLocalidad = new Localidad()
            {
                Pais = "Costa Rica",
                Estado = "Cartago",
                Ciudad = "Tejar"
            };

            _localidadDA.insertLocalidad(_newLocalidad);

            List<Localidad> _localidades = _localidadDA.getLocalidades(100);

            foreach(var localidad in _localidades)
            {
                Console.WriteLine(localidad.idLocalidad + "-" + localidad.Pais + "-" + localidad.Estado + "-" + localidad.Ciudad);
            }
            Console.Read();


        }
    }
}
