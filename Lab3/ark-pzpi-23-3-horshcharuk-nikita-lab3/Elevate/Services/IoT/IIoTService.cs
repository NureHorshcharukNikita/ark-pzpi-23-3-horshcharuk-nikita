using Elevate.Dtos.IoT;
using Elevate.Dtos.Teams;

namespace Elevate.Services.IoT;

public interface IIoTService
{
    Task<IotScanResultDto> ProcessScanAsync(
        string deviceKey,
        int userId,
        CancellationToken cancellationToken);

    Task<DeviceScanResponseDto> ProcessScanAsync(
        DeviceScanRequestDto dto,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<LeaderboardEntryDto>> GetLeaderboardAsync(
        int teamId,
        CancellationToken cancellationToken);
}
