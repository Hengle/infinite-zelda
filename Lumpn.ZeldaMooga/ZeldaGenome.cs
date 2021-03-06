﻿using System.Collections.Generic;
using Lumpn.Mooga;
using Lumpn.ZeldaDungeon;
using Lumpn.Utils;

namespace Lumpn.ZeldaMooga
{
    public sealed class ZeldaGenome : Genome
    {
        private static readonly ZeldaGeneFactory factory = new ZeldaGeneFactory();

        private readonly ZeldaConfiguration configuration;
        private readonly List<ZeldaGene> genes;

        public ZeldaGenome(ZeldaConfiguration configuration)
        {
            this.configuration = configuration;

            // add some more genes
            int count = configuration.CalcNumInitialGenes();
            this.genes = GeneUtils.Generate(count, factory, configuration);
        }

        private ZeldaGenome(ZeldaConfiguration configuration, List<ZeldaGene> genes)
        {
            this.configuration = configuration;
            this.genes = genes;
        }

        public Pair<Genome> Crossover(Genome o, RandomNumberGenerator random)
        {
            ZeldaGenome other = (ZeldaGenome)o;

            // randomly distribute genes
            var newGenesA = new List<ZeldaGene>(genes);
            var newGenesB = new List<ZeldaGene>(other.genes);
            GeneUtils.Crossover(genes, other.genes, random);

            // assemble offsprings
            var a = new ZeldaGenome(configuration, newGenesA);
            var b = new ZeldaGenome(configuration, newGenesB);
            return new Pair<Genome>(a, b);
        }

        public Genome Mutate(RandomNumberGenerator random)
        {
            // mutate genes
            var newGenes = new List<ZeldaGene>(genes);
            GeneUtils.Mutate(newGenes, factory, configuration);

            // assemble offspring
            return new ZeldaGenome(configuration, newGenes);
        }

        public void Express(ZeldaDungeonBuilder builder)
        {
            foreach (ZeldaGene gene in genes)
            {
                gene.Express(builder);
            }
        }

        public override string ToString()
        {
            return string.Join(", ", genes);
        }
    }
}
