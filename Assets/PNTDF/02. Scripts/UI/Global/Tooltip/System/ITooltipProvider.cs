namespace PNTD
{
    public interface ITooltipProvider
    {
        TooltipContent GetTooltipContent();
        bool CanShowTooltip { get; }
    }
}