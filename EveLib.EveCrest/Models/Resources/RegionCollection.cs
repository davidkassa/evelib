﻿// ***********************************************************************
// Assembly         : EveLib.EveCrest
// Author           : Lars Kristian
// Created          : 12-16-2014
//
// Last Modified By : Lars Kristian
// Last Modified On : 12-17-2014
// ***********************************************************************
// <copyright file="RegionCollection.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Runtime.Serialization;
using eZet.EveLib.EveCrestModule.Models.Links;

namespace eZet.EveLib.EveCrestModule.Models.Resources {
    /// <summary>
    ///     Class RegionCollection. This class cannot be inherited.
    /// </summary>
    [DataContract]
    public sealed class RegionCollection : CollectionResource<RegionCollection, LinkedEntity<Region>> {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RegionCollection" /> class.
        /// </summary>
        public RegionCollection() {
            ContentType = "application/vnd.ccp.eve.RegionCollection-v1+json";
        }
    }
}