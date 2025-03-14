﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DataModels
{
    public class Tasks
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Subject { get; set; }
        public bool IsComplete { get; set; }
        public Guid? AssignedMemberId { get; set; }
    }
}
