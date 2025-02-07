using MusicEShop.Domain.DomainModels;
using MusicEShop.Repository.Interface;
using MusicEShop.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicEShop.Service.Implementation
{
    public class TrackService : ITrackService
    {
        private readonly IRepository<Track> _trackRepository;

        public TrackService(IRepository<Track> trackRepository)
        {
            _trackRepository = trackRepository;
        }

        public void CreateTrack(Track track)
        {
            _trackRepository.Insert(track);
        }

        public void DeleteTrack(Guid id)
        {
            var track = _trackRepository.GetById(id);
            _trackRepository.Delete(track);
        }

        public List<Track> GetAllTracks()
        {
            return _trackRepository.GetAll().ToList();
        }

        public Track GetTrackById(Guid id)
        {
            var track = _trackRepository.GetById(id);
            return track;
        }

        public void UpdateTrack(Track track)
        {
            _trackRepository.Update(track);
        }
    }
}
