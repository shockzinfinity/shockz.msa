using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shockz.msa.movie.api.Models;

namespace shockz.msa.movie.api.Data
{
    public class MoviesAPIContext : DbContext
    {
        public MoviesAPIContext (DbContextOptions<MoviesAPIContext> options)
            : base(options)
        {
        }

        public DbSet<shockz.msa.movie.api.Models.Movie> Movies { get; set; } = default!;
    }
}
