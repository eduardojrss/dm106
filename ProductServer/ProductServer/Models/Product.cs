using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace ProductServer.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage= "O	campo	nome	é	obrigatório")]
        public string nome { get; set; }
        public string descricao { get; set; }
        [Required]
        [StringLength(8, ErrorMessage = "O	tamanho	máximo	do	c ódigo	é	8	caracteres")]
        public string codigo { get; set; }
        [Range(10, 999, ErrorMessage = "O	preço	deverá	ser	ent re	10	e	999.")]
        public decimal preco { get; set; }
        [StringLength(80, ErrorMessage = "O	tamanho	máximo	da	url	é	80	caracteres")]
        public string Url { get; set; }
        public string cor { get; set; }
        public string modelo { get; set; }
        public decimal altura { get; set; }
        public decimal largura { get; set; }
        public decimal comprimento { get; set; }
        public decimal diametro { get; set; }
    }
}