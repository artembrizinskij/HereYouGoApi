using System;
using Domain.Abstractions;

namespace Domain.Entities
{
    public class Entity : IEntity
    {
        public Guid Id { get; set; }
    }
}
