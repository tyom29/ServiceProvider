using ServiceProvider.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider.Repositories
{
    public interface ICustomerRepository
    {
        void SaveCustomer(Customer customer);
        Customer GetCustomer(string email);

    }

}
