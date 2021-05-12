﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace finance_app.Types.Responses
{
    public class ListResponse<T>
    {
        public ListResponse(IList<T> items) {
            Items = items;
            Length = items.Count;
        }

        public int? Length { get; set; }
        public IEnumerable<T> Items { get; set; } 
    }
}