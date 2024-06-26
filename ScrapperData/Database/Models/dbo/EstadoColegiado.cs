﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ScrapperData.Database.Models;

[Table("EstadoColegiado")]
[Index("Descripcion", Name = "IX_EstadoColegiado")]
public partial class EstadoColegiado
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(250)]
    public string Descripcion { get; set; }

    public int Estado { get; set; }

    [InverseProperty("CondicionColegiado")]
    public virtual ICollection<Colegiado> Colegiados { get; set; } = new List<Colegiado>();
}