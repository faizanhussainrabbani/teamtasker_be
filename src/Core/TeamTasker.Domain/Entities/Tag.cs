using System;
using TeamTasker.Domain.Events;
using TeamTasker.SharedKernel;

namespace TeamTasker.Domain.Entities
{
    /// <summary>
    /// Tag entity representing a tag that can be associated with tasks
    /// </summary>
    public class Tag : BaseEntity
    {
        private Tag() { } // Required by EF Core

        public Tag(string name, string color = "#808080")
        {
            Name = name;
            Color = color;
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
            
            AddDomainEvent(new TagCreatedEvent(this));
        }

        public string Name { get; private set; }
        public string Color { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime UpdatedDate { get; private set; }
        
        // Navigation properties
        public ICollection<TaskTag> TaskTags { get; private set; } = new List<TaskTag>();

        public void Update(string name, string color)
        {
            Name = name;
            Color = color;
            UpdatedDate = DateTime.UtcNow;
            
            AddDomainEvent(new TagUpdatedEvent(this));
        }
    }
}
