using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsGoiaService.Models
{
    class ProduccionViewModel
    {
        /// <summary>
        /// Identificador del cultivo según GOIA
        /// </summary>
        public int cultivo { get; set; }
        /// <summary>
        /// Fecha del vale de producción
        /// </summary>
        public DateTime fecha { get; set; }
        /// <summary>
        /// Código de la explotación según la cooperativa
        /// </summary>
        public string codfinca { get; set; }
        /// <summary>
        /// Código de la parcela según la cooperativa
        /// </summary>
        public string codparcela { get; set; }
        /// <summary>
        /// Código de la variedad según la cooperativa
        /// </summary>
        public string codvariedad { get; set; }
        /// <summary>
        /// Número de unidades (cajas, sacos, piñas,...) en las que se transporta el producto
        /// </summary>
        public int unidades { get; set; }
        /// <summary>
        /// Número de kilos totales del producto
        /// </summary>
        public int kilos { get; set; }
        /// <summary>
        /// Número del vale de producción para la cooperativa
        /// </summary>
        public string numvale { get; set; }
    }
}
