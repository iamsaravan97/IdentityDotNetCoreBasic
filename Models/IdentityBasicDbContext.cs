using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityBasicDotNetCore.Models
{
    public class IdentityBasicDbContext : IdentityDbContext
    {
        public IdentityBasicDbContext(DbContextOptions options) : base(options)
        {
                
        }
    }
}
