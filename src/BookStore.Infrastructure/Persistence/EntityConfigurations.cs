using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Persistence;

public class LivroConfiguration : IEntityTypeConfiguration<Livro>
{
    public void Configure(EntityTypeBuilder<Livro> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .ValueGeneratedNever();

        builder.Property(l => l.Titulo)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(l => l.Autor)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(l => l.AnoPub)
            .IsRequired(false);

        builder.Property(l => l.QtdeDisponivel)
            .IsRequired();

        builder.Property(l => l.DataCriacao)
            .IsRequired();

        builder.Property(l => l.DataUltimaAtualizacao)
            .IsRequired(false);

        builder.ToTable("Livros");
    }
}

public class EmprestimoConfiguration : IEntityTypeConfiguration<Emprestimo>
{
    public void Configure(EntityTypeBuilder<Emprestimo> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.LivroId)
            .IsRequired();

        builder.Property(e => e.DataEmprestimo)
            .IsRequired();

        builder.Property(e => e.DataDevolucao)
            .IsRequired(false);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(e => e.DataCriacao)
            .IsRequired();

        builder.Property(e => e.DataUltimaAtualizacao)
            .IsRequired(false);

        builder.HasOne<Livro>()
            .WithMany()
            .HasForeignKey(e => e.LivroId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Emprestimos");
    }
}
