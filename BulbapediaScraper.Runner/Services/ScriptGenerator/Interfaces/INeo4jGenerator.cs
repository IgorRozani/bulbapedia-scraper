﻿using BulbapediaScraper.Runner.Services.ScriptGenerator.Model;
using System.Collections.Generic;

namespace BulbapediaScraper.Runner.Services.ScriptGenerator.Interfaces
{
    public interface INeo4jGenerator
    {
        string CreateNodes(IList<Node> nodes);

        string CreateNode(Node node);

        string CreateRelationships(IList<Relationship> relationships);

        string CreateRelationship(Relationship relationship);
    }
}
