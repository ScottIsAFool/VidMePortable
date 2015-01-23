using PropertyChanged;

namespace VidMePortable.Model.Requests
{
    [ImplementPropertyChanged]
    public class AppRequest
    {
        public string Name { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        public string Organisation { get; set; }
        public string RedirectUri { get; set; }
    }
}
