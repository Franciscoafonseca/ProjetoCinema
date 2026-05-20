using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using OnlineCinemaFestival.Api.Data;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260520170000_ExpandirPerfilAutenticacao")]
    partial class ExpandirPerfilAutenticacao
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "10.0.7");
        }
    }
}
