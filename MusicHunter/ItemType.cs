﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Uthef.MusicHunter
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ItemType
    {
        Track, Album
    }
}
