﻿using GymSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Specifications.EquipmentSpec
{
    public class EquipmentWithRelationsSpecification : BaseSpecification<Equipment>
    {
        public EquipmentWithRelationsSpecification()
        {
            AddIncludes(e => e.MaintainedByUsers);
            AddIncludes(e => e.UsedInClasses);
        }

        public EquipmentWithRelationsSpecification(Expression<Func<Equipment, bool>> criteria) : base(criteria)
        {
            AddIncludes(e => e.MaintainedByUsers);
            AddIncludes(e => e.UsedInClasses);
        }
    }
}
