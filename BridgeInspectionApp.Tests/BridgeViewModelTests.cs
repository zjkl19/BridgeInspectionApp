using BridgeInspectionApp.Data;
using BridgeInspectionApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeInspectionApp.Tests;

public class BridgeViewModelTests
{
    [Fact]
    public async Task AddBridge_WithValidData_AddsBridge()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<BridgeContext>()
            .UseInMemoryDatabase(databaseName: "AddBridgeTestDb")
            .Options;


        var bridgeViewModel = new BridgeViewModel
        {
            Name = "New Bridge",
            Location = "New Location",
            MapId = "Map123"
        };

        using var context = new BridgeContext(options);
        //var command = new BridgeCommands(context); // Assuming your ExecuteAddConfirmedCommand method is in this class

        // Act
        await bridgeViewModel.ExecuteAddConfirmedCommand(bridgeViewModel);

        // Assert
        var bridgeExists = await context.Bridges.AnyAsync(b => b.Name == "New Bridge");
        Assert.True(bridgeExists);
    }
}
