using FollwItPortable.Attributes;

namespace FollwItPortable.Model
{
    public enum FollwItGenre
    {
        Action,
        Adult,
        Adventure,
        Animation,
        Biography,
        Comedy,
        Crime,
        Disaster,
        Documentary,
        Drama,
        Eastern,
        Family, 
        Fantasy,
        [Description("film-noir")]
        FilmNoir,
        [Description("game-show")]
        GameShow,
        History,
        Holiday,
        Horror,
        Music,
        Musical,
        Mystery,
        [Description("reality-tv")]
        RealityTv,
        [Description("road-movie")]
        RoadMovie,
        Romance,
        [Description("science-fiction")]
        ScienceFiction,
        Sport,
        Suspense,
        [Description("talk-show")]
        TalkShow,
        Thriller,
        War,
        Western
    }
}