using ServiceProvider.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceProvider.Models;
using System.Data.Entity.Validation;

namespace ServiceProvider.Repositories
{
    public class CustomerRepository:ICustomerRepository
    {
        private readonly ApplicationDbContext _customerContext;
        public CustomerRepository(ApplicationDbContext customerContext)
        {
            _customerContext = customerContext;
        }

        /// <summary>
        ///  Function that will save our Customer itno Database Customers Table.
        /// </summary>
        /// <param name="customer"></param>
        public void SaveCustomer(Customer customer)
        {
            try
            {
                _customerContext.Customers.Add(customer);
                _customerContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;


            }

        }

       public Customer GetCustomer(string email)
        {
            
            Customer customer = _customerContext.Customers.SingleOrDefault(customers => customers.Email == email);

            return customer;
        }
    }
}