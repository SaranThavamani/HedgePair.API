using HedgePair.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HedgePair.API.Data;

/// <summary>EF Core DbContext for the Hedge Pair Management System.</summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<FinancialInstrument> FinancialInstruments { get; set; } = null!;
    public DbSet<Models.HedgePair> HedgePairs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── FinancialInstrument ──────────────────────────────────────────────
        modelBuilder.Entity<FinancialInstrument>(e =>
        {
            e.ToTable("Financial_Instrument");
            e.HasKey(f => f.FinId);

            e.Property(f => f.FinId)
             .HasColumnName("FIN_ID")
             .ValueGeneratedOnAdd();

            e.Property(f => f.DealNumber)
             .HasColumnName("Deal_Number")
             .HasMaxLength(10)
             .IsRequired();

            e.Property(f => f.NotionalAmt)
             .HasColumnName("Notional_Amt")
             .HasColumnType("decimal(18,2)")
             .IsRequired();

            e.Property(f => f.DealType)
             .HasColumnName("deal_type")
             .HasMaxLength(10)
             .IsRequired();

            e.Property(f => f.DealStatus)
             .HasColumnName("deal_status")
             .HasMaxLength(10)
             .IsRequired();

            e.HasIndex(f => f.DealNumber).IsUnique();

            // Relationship: one FI can be the Bond side of at most one HedgePair
            e.HasOne(f => f.BondPair)
             .WithOne(hp => hp.Bond)
             .HasForeignKey<Models.HedgePair>(hp => hp.BondFinId)
             .OnDelete(DeleteBehavior.Restrict);

            // Relationship: one FI can be the Swap side of at most one HedgePair
            e.HasOne(f => f.SwapPair)
             .WithOne(hp => hp.Swap)
             .HasForeignKey<Models.HedgePair>(hp => hp.SwapFinId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── HedgePair ────────────────────────────────────────────────────────
        modelBuilder.Entity<Models.HedgePair>(e =>
        {
            e.ToTable("Hedge_Pair");
            e.HasKey(hp => hp.HedgePairId);

            e.Property(hp => hp.HedgePairId)
             .HasColumnName("HEDGE_PAIR_ID")
             .ValueGeneratedOnAdd();

            e.Property(hp => hp.BondFinId)
             .HasColumnName("BOND_FIN_ID")
             .IsRequired();

            e.Property(hp => hp.SwapFinId)
             .HasColumnName("SWAP_FIN_ID")
             .IsRequired();

            e.Property(hp => hp.BondNotionalAmt)
             .HasColumnName("BOND_NOTIONAL_AMT")
             .HasColumnType("decimal(18,2)")
             .IsRequired();

            e.Property(hp => hp.SwapNotionalAmt)
             .HasColumnName("SWAP_NOTIONAL_AMT")
             .HasColumnType("decimal(18,2)")
             .IsRequired();

            e.HasIndex(hp => hp.BondFinId).IsUnique();
            e.HasIndex(hp => hp.SwapFinId).IsUnique();
        });

        // ── Seed Data ────────────────────────────────────────────────────────
        modelBuilder.Entity<FinancialInstrument>().HasData(
            new FinancialInstrument { FinId = 1, DealNumber = "DL1234567", NotionalAmt = 100000.50m, DealType = "BOND", DealStatus = "Active" },
            new FinancialInstrument { FinId = 2, DealNumber = "DL2345678", NotionalAmt = 200000.75m, DealType = "BOND", DealStatus = "Active" },
            new FinancialInstrument { FinId = 3, DealNumber = "DL3456789", NotionalAmt = 150000.00m, DealType = "BOND", DealStatus = "Active" },
            new FinancialInstrument { FinId = 4, DealNumber = "DL4567890", NotionalAmt = 250000.25m, DealType = "BOND", DealStatus = "Active" },
            new FinancialInstrument { FinId = 5, DealNumber = "DL5678901", NotionalAmt = 300000.10m, DealType = "BOND", DealStatus = "Inactive" },
            new FinancialInstrument { FinId = 6, DealNumber = "DL1234568", NotionalAmt = 100000.50m, DealType = "SWAP", DealStatus = "Active" },
            new FinancialInstrument { FinId = 7, DealNumber = "DL2345679", NotionalAmt = 200000.75m, DealType = "SWAP", DealStatus = "Active" },
            new FinancialInstrument { FinId = 8, DealNumber = "DL3456780", NotionalAmt = 150000.00m, DealType = "SWAP", DealStatus = "Active" },
            new FinancialInstrument { FinId = 9, DealNumber = "DL4567810", NotionalAmt = 250000.25m, DealType = "SWAP", DealStatus = "Active" },
            new FinancialInstrument { FinId = 10, DealNumber = "DL5678911", NotionalAmt = 300000.10m, DealType = "SWAP", DealStatus = "Inactive" }
        );
    }
}
