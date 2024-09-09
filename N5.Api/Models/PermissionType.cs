﻿namespace N5.Api.Models
{
    public class PermissionType
    {
        public int Id { get; set; }
        public string? Description { get; set; }

        public ICollection<Permission>? Permissions { get; set; }
    }
}
