﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkExercise.Models;

[Table("customer")]
public class Customer
{
    [Key]
    [Column("id")]
    public int Id { get; set; } 

    [Column("name")]
    public string Name { get; set; } = string.Empty!;

    public List<Sale> Sales { get; set; } = default!;
}