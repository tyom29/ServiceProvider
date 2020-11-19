using ServiceProvider.Domain;
using ServiceProvider.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceProvider.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        public Customer GetCustomer(string email)
        {

            return _customerRepository.GetCustomer(email);
        }

        public  void SaveCustomer(Customer customer)
        {
            _customerRepository.SaveCustomer(customer);
        }
    }
}