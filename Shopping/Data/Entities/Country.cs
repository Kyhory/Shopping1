﻿using System.ComponentModel.DataAnnotations;

namespace Shopping.Data.Entities
{
    public class Country
    {
        public int Id { get; set; }

        [Display(Name="País")]
        [MaxLength(50, ErrorMessage = "El campo {0} no debe tener máximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio." )]
        public string Name { get; set; }

        public ICollection<State> States { get; set; }

        [Display(Name = "Estados/Departamentos")]
        public int StatesNumber => States == null ? 0 : States.Count;

        [Display(Name = "Ciudades")]
        public int CitiesNumber => States == null ? 0 : States.Sum(s => s.CitiesNumber);

    }
}
