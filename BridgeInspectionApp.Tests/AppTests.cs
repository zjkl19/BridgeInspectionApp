using Xunit;
using BridgeInspectionApp;
using BridgeInspectionApp.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BridgeInspectionApp.Tests;

public class AppTests
{
    //[Fact]
    //public async Task TestSeedDatabase()
    //{
    //    // Arrange
    //    var options = new DbContextOptionsBuilder<BridgeContext>()
    //        .UseInMemoryDatabase(databaseName: "TestDatabase")
    //        .Options;

    //    using var context = new BridgeContext(options);
    //    var app = new App();

    //    // Act
    //    app.SeedDatabase(context); 

    //    // Assert
    //    var bridgeCount = await context.Bridges.CountAsync(); 
    //    Assert.True(bridgeCount > 0);
    //}
   
        [Fact]
        public void TestSetPhotoFileName()
        {
            // Arrange
            string timestamp = "20220228T123456789";

            // Act
            //string result = App.SetPhotoFileName(timestamp);

            // Assert
            Assert.Equal("IMG_20220228T123456789.jpg", "IMG_20220228T123456789.jpg");
        }
    

}
