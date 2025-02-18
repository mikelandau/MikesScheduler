using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MikesScheduler.Repositories
{
    public interface IAuthRepository
    {
        public Task<Auth> Login(string username, string password);
    }
}
