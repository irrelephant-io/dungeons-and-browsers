namespace Irrelephant.DnB.Core.Infrastructure
{
    public interface ICopyable<TCopied>
    {
        TCopied Copy();
    }
}
