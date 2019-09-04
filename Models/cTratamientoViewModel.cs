using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsGoiaService.Models
{
    class TratamientoViewModel
    {
        public int? tratamientoid { get; set; }
        public int? parcelaid { get; set; }
        public int? fincaid { get; set; }
        public DateTime? fecha_alta { get; set; }
        public int? cultivoid { get; set; }
        public int? recomendacionid { get; set; }
        public string userid { get; set; }
        public DateTime? fecha_ini { get; set; }
        public DateTime? fecha_fin { get; set; }
        public int? huertaid { get; set; }
        public int? variedadid { get; set; }
        public string nombre { get; set; }
        public int? maquinaid { get; set; }
        public string aplicador { get; set; }
        public Decimal? cantidad { get; set; }
        public string formato { get; set; }
        public string firmado { get; set; }
        public Decimal? sobrante { get; set; }
        public int? max_plazo_seguridad { get; set; }
        public int? dias_restantes { get; set; }
        public string plagas { get; set; }
        public int? cooperativaid { get; set; }
        public Boolean? muestreada { get; set; }
        public string nombre_finca { get; set; }
        public string codigo_finca { get; set; }
        public string codproductor { get; set; }
        public string nombre_productor { get; set; }
        public string nif_productor { get; set; }
        public string nombre_huerta { get; set; }
        public string nombre_usuario { get; set; }
        public string apellidos_usuario { get; set; }
        public string imagenurl { get; set; }
        public string nombre_parcela { get; set; }
    }
}
