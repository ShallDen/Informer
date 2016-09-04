// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LastUpdate.cs" company="Joan Caron">
// Copyright (c) 2014 All Rights Reserved
// </copyright>
// <author>Joan Caron</author>
// <summary>Implements the last update class</summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OpenWeatherMap
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;
    using System.Data.Entity;

    /// <summary>
    ///     Class LastUpdate.
    /// </summary>
    public sealed class LastUpdate
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        [XmlAttribute("value")]
        public DateTime Value { get; set; }
    }
}
