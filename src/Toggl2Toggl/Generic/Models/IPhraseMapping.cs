namespace Toggl2Toggl
{
    public interface IPhraseMapping<T>
    {
        bool TryGet(string phrase, out T value);

        void Map(T key, string phrase);
    }
}