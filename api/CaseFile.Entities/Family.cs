﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CaseFile.Entities
{
    public partial class Family
    {        
        public int FamilyId { get; set; }

        public string Name { get; set; }
    }
}
