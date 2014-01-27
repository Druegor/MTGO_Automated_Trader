using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CardDataLayer.Models;
using Dapper;
using Framework.Interfaces;

namespace CardDataLayer.Repositories
{
	public class PixelLocationsRepository
	{
		private readonly IConnection _connection;

		public PixelLocationsRepository(IConnection connection)
		{
			_connection = connection;
		}

		public List<PixelLocation> Get()
		{
			var resolution = string.Format("{0}x{1}", SystemInformation.PrimaryMonitorSize.Width,
											  SystemInformation.PrimaryMonitorSize.Height);

			var operatingSystem = string.Format("{0}.{1}", Environment.OSVersion.Version.Major,
												   Environment.OSVersion.Version.Minor);

			return _connection.GetConnection().Query<PixelLocation>(
				"SELECT * " +
				"FROM PixelLocations " +
				"WHERE OperatingSystemId = @operatingSystem " +
				"AND ResolutionId = @resolution",
				new { operatingSystem, resolution },
				_connection.GetTransaction()
				).ToList();
		}
	}
}
