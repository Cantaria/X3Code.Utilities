﻿using System;
using System.ComponentModel.DataAnnotations;

namespace X3Code.Infrastructure.Mobile
{
	public abstract class Entity : IEntity
	{
		[Key]
		public abstract string EntityId { get; set; }
	}
}