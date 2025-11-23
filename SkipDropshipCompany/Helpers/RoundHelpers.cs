using System.Linq;
using BepInEx.Logging;
using SkipDropshipCompany.Utils;

namespace SkipDropshipCompany.Helpers;

internal static class RoundHelpers
{
    internal static ManualLogSource Logger => SkipDropshipCompany.Logger;

    public static bool IsInFirstDayOrbit()
    {
        if (!RoundUtils.IsInOrbit())
        {
            // Landed
            Logger.LogDebug("Not in orbit.");
            return false;
        }

        if (!RoundUtils.IsFirstDay())
        {
            // Not first day
            Logger.LogDebug("Not first day.");
            return false;
        }

        return true;
    }

    public static bool IsInFirstDayOrbitAndRoutingToCompany()
    {
        if (!IsInFirstDayOrbit())
        {
            // Not in first day orbit
            Logger.LogDebug("Not in first day orbit.");
            return false;
        }

        if (!RoundUtils.IsRoutingToCompany())
        {
            // Not routing to company
            Logger.LogDebug("Not routing to company.");
            return false;
        }

        return true;
    }

    public static bool IsLandedOnCompany()
    {
        if (RoundUtils.IsInOrbit())
        {
            // In orbit
            Logger.LogDebug("In orbit.");
            return false;
        }

        if (!RoundUtils.IsRoutingToCompany())
        {
            // Not routing to company
            Logger.LogDebug("Not routing to company.");
            return false;
        }

        return true;
    }

    public static bool IsInOrbitAndLastLandedOnCompanyAndRoutingToCompany()
    {
        if (!RoundUtils.IsInOrbit())
        {
            // Landed
            Logger.LogDebug("Not in orbit.");
            return false;
        }

        if (!LandingHistoryHelpers.IsLastLandedOnCompany())
        {
            // Last landed level is not company
            Logger.LogDebug("Last landed level is not company.");
            return false;
        }

        if (!RoundUtils.IsRoutingToCompany())
        {
            // Not routing to company
            Logger.LogDebug("Not routing to company.");
            return false;
        }

        return true;
    }
}
