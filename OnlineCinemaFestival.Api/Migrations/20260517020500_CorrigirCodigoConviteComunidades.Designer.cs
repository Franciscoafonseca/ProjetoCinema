using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using OnlineCinemaFestival.Api.Data;

#nullable disable

namespace OnlineCinemaFestival.Api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260517020500_CorrigirCodigoConviteComunidades")]
    partial class CorrigirCodigoConviteComunidades
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "10.0.7");
#pragma warning restore 612, 618
        }
    }
}
