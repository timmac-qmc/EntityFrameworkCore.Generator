﻿using EntityFrameworkCore.Generator.Extensions;
using EntityFrameworkCore.Generator.Metadata.Generation;
using Microsoft.EntityFrameworkCore.Internal;

namespace EntityFrameworkCore.Generator.Templates
{
    public class DataContextTemplate : CodeTemplateBase
    {
        private readonly EntityContext _entityContext;

        public DataContextTemplate(EntityContext entityContext)  
        {
            _entityContext = entityContext;
        }
        
        public override string WriteCode()
        {
            CodeBuilder.Clear();

            CodeBuilder.AppendLine("using System;");
            CodeBuilder.AppendLine("using Microsoft.EntityFrameworkCore;");
            CodeBuilder.AppendLine("using Microsoft.EntityFrameworkCore.Metadata;");
            CodeBuilder.AppendLine();

            CodeBuilder.AppendLine($"namespace {_entityContext.ContextNamespace}");
            CodeBuilder.AppendLine("{");

            using (CodeBuilder.Indent())
            {
                GenerateClass();
            }

            CodeBuilder.AppendLine("}");

            return CodeBuilder.ToString();
        }


        private void GenerateClass()
        {
            var contextClass = _entityContext.ContextClass.ToSafeName();
            var baseClass = _entityContext.ContextBaseClass.ToSafeName();

            CodeBuilder.AppendLine("/// <summary>");
            CodeBuilder.AppendLine("/// A <see cref=\"DbContext\" /> instance represents a session with the database and can be used to query and save instances of entities. ");
            CodeBuilder.AppendLine("/// </summary>");

            CodeBuilder.AppendLine($"public partial class {contextClass} : {baseClass}");
            CodeBuilder.AppendLine("{");

            using (CodeBuilder.Indent())
            {
                GenerateConstructors();
                GenerateDbSets();
                GenerateOnConfiguring();
            }

            CodeBuilder.AppendLine("}");
        }

        private void GenerateConstructors()
        {
            var contextName = _entityContext.ContextClass.ToSafeName();

            CodeBuilder.AppendLine("/// <summary>");
            CodeBuilder.AppendLine($"/// Initializes a new instance of the <see cref=\"{contextName}\"/> class.");
            CodeBuilder.AppendLine("/// </summary>");
            CodeBuilder.AppendLine("/// <param name=\"options\">The options to be used by this <see cref=\"DbContext\" />.</param>");


            CodeBuilder.AppendLine($"public {contextName}(DbContextOptions<{contextName}> options)")
                .IncrementIndent()
                .AppendLine(": base(options)")
                .DecrementIndent()
                .AppendLine("{")
                .AppendLine("}")
                .AppendLine();
        }

        private void GenerateDbSets()
        {
            CodeBuilder.AppendLine("#region Generated Properties");
            foreach (var entityType in _entityContext.Entities)
            {
                var entityClass = entityType.EntityClass.ToSafeName();
                var propertyName = entityType.ContextProperty.ToSafeName();
                var safeName = $"{entityType.EntityNamespace}.{entityClass}";

                CodeBuilder.AppendLine("/// <summary>");
                CodeBuilder.AppendLine($"/// Gets or sets the <see cref=\"T:Microsoft.EntityFrameworkCore.DbSet`1\" /> that can be used to query and save instances of <see cref=\"{safeName}\"/>.");
                CodeBuilder.AppendLine("/// </summary>");
                CodeBuilder.AppendLine("/// <value>");
                CodeBuilder.AppendLine($"/// The <see cref=\"T:Microsoft.EntityFrameworkCore.DbSet`1\" /> that can be used to query and save instances of <see cref=\"{safeName}\"/>.");
                CodeBuilder.AppendLine("/// </value>");

                CodeBuilder.AppendLine($"public virtual DbSet<{safeName}> {propertyName} {{ get; set; }}");
                CodeBuilder.AppendLine();
            }

            CodeBuilder.AppendLine("#endregion");

            if (_entityContext.Entities.Any())
                CodeBuilder.AppendLine();
        }

        private void GenerateOnConfiguring()
        {
            CodeBuilder.AppendLine("/// <summary>");
            CodeBuilder.AppendLine("/// Configure the model that was discovered from the entity types exposed in <see cref=\"T:Microsoft.EntityFrameworkCore.DbSet`1\" /> properties on this context.");
            CodeBuilder.AppendLine("/// </summary>");
            CodeBuilder.AppendLine("/// <param name=\"modelBuilder\">The builder being used to construct the model for this context.</param>");

            CodeBuilder.AppendLine("protected override void OnModelCreating(ModelBuilder modelBuilder)");
            CodeBuilder.AppendLine("{");

            using (CodeBuilder.Indent())
            {
                CodeBuilder.AppendLine("#region Generated Configuration");
                foreach (var entityType in _entityContext.Entities)
                {
                    var mappingClass = entityType.MappingClass.ToSafeName();

                    CodeBuilder.AppendLine($"modelBuilder.ApplyConfiguration(new {entityType.MappingNamespace}.{mappingClass}());");
                }

                CodeBuilder.AppendLine("#endregion");
            }

            CodeBuilder.AppendLine("}");
        }
    }
}