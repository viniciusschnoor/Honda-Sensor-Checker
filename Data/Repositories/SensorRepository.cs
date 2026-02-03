using HondaSensorChecker.Data.Context;
using HondaSensorChecker.Data.Interfaces;
using HondaSensorChecker.Models;
using Microsoft.EntityFrameworkCore;

namespace HondaSensorChecker.Data.Repositories
{
    public class SensorRepository : RepositoryBase<Sensor>, ISensorRepository
    {
        private readonly DbSet<Sensor> _dbSetSensor;
        public SensorRepository(DataContext context) : base(context)
        {
            _dbSetSensor = context.Set<Sensor>();
        }
    }
}