using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ProductServer.Models;
using ProductServer.br.com.correios.ws;
using ProductServer.CRMClient;

namespace ProductServer.Controllers
{
    [Authorize]
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private ProductServerContext db = new ProductServerContext();

        [ResponseType(typeof(string))]
        [HttpGet]
        [Route("frete")]
        public IHttpActionResult CalculaFrete()
        {
            string frete;
            CalcPrecoPrazoWS correios = new CalcPrecoPrazoWS();
            cResultado resultado = correios.CalcPrecoPrazo("", "", "40010", "37540000", "37002970", "1", 1, 30, 30, 30, 30, "N",	100,	"S");

            if (resultado.Servicos[0].Erro.Equals("0")) {
                frete = "Valor	do	frete:	" + resultado.Servicos[0].Valor + "	-	Prazo	de	entrega:	" + resultado.Servicos[0].PrazoEntrega + "	dia(s)";
                return Ok(frete);
            } else {
                return BadRequest("Código	do	erro:	" + resultado.Servicos[0].Erro + "-" + resultado.Servicos[0].MsgErro);
            }
        }

        [ResponseType(typeof(string))]
        [HttpGet]
        [Route("cep")]
        public IHttpActionResult ObtemCEP()
        {
            CRMRestClient crmClient = new CRMRestClient();
            Customer customer = crmClient.GetCustomerByEmail(User.Identity.Name);
            if (customer != null) {
                return Ok(customer.zip);
            } else {
                return BadRequest("Falha ao	consultar o CRM");
            }
        }


        // GET: api/Orders
        [Authorize(Roles = "ADMIN")]
        public List<Order> GetOrders()
        {
            return db.Orders.ToList<Order>();
        }

        [Authorize(Roles = "ADMIN, USER")]
        public List<Order> GetOrders(string email)
        {
            List<Order> orders = db.Orders.Where(o => o.email == email).ToList<Order>();

            string emailLogin = User.Identity.Name;

            if (emailLogin.Equals("admin@admin.com"))
            {
                return orders;
            }
            else
            {
                if (emailLogin.Equals(email))
                {
                    return orders;
                }
                else
                {
                    return null;
                }
            }
        }

        // GET: api/Orders/5
        [Authorize(Roles = "ADMIN, USER")]
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            string email = User.Identity.Name;

            if (email.Equals("admin@admin.com"))
            {
                return Ok(order);
            }
            else {
                if (email.Equals(order.email))
                {
                    return Ok(order);
                }
                else {
                    return BadRequest("Permissão negada");
                }
            }
        }

        // PUT: api/Orders/5
        [Authorize(Roles = "ADMIN")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Orders
        [Authorize(Roles = "ADMIN")]
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            order.status = "novo";
            order.precoPedido = 0;
            order.precoFrete = 0;
            order.peso = 0;
            order.dataPedido = DateTime.Now;
            db.Orders.Add(order);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = order.Id }, order);
        }

        // PUT: api/Orders/fechar/5
        [Authorize(Roles = "ADMIN, USER")]
        [ResponseType(typeof(void))]
        [Route("fechar")]
        public IHttpActionResult fechar(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            if (id != order.Id)
            {
                return BadRequest();
            }

            if (order.precoFrete == 0) {
                return BadRequest("Frete ainda não calculado");
            }

            order.status = "fechado";
            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            string email = User.Identity.Name;

            if (email.Equals("admin@admin.com"))
            {
                db.Orders.Remove(order);
                db.SaveChanges();
                return Ok(order);
            }
            else
            {
                if (email.Equals(order.email))
                {
                    db.Orders.Remove(order);
                    db.SaveChanges();
                    return Ok(order);
                }
                else
                {
                    return BadRequest("Permissão negada");
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.Id == id) > 0;
        }
    }
}