using System.Collections.Generic;
using NUnit.Framework;
using Sonneville.AssessorsAdapter.Scraper.Assessors;
using Sonneville.AssessorsAdapter.Scraper.CSV.Polk;

namespace Sonneville.AssessorsAdapter.Scraper.Test.CSV.Polk
{
    [TestFixture]
    public class PolkCountyHouseDataTests
    {
        [SetUp]
        public void Setup()
        {
            _realEstateRecord = new RealEstateRecord
            {
                Assessments = new List<Assessment>
                {
                    new Assessment
                    {
                        Building = 10,
                        Land = 5,
                    },
                },
                Land = new LandRecord
                {
                    SquareFeet = 9000,
                },
                Location = new LocationRecord
                {
                    Address = "1234 main street",
                    City = "my city",
                    Zip = 12345,
                    County = "Polk",
                    State = "Iowa",
                },
                Residence = new ResidenceRecord
                {
                    Bedrooms = 3,
                    Bathrooms = 2,
                    Condition = "4+00",
                    YearBuilt = 2000,
                    LivingAreaSquareFootage = new Dictionary<int, int?>
                    {
                        {-1, 1000},
                        {0, 1000},
                        {1, 400},
                    },
                    UnfinishedBasementSquareFootage = 1000,
                },
            };
        }

        private RealEstateRecord _realEstateRecord;

        [Test]
        public void ShouldSetAddress()
        {
            var polkCountyHouseData = PolkCountyHouseData.CreateFrom(_realEstateRecord);

            Assert.AreEqual(_realEstateRecord.Location.Address, polkCountyHouseData.StreetAddress);
        }

        [Test]
        public void ShouldSetAssessmentValue()
        {
            var polkCountyHouseData = PolkCountyHouseData.CreateFrom(_realEstateRecord);

            Assert.AreEqual(_realEstateRecord.CurrentValue, polkCountyHouseData.AssessmentValue);
        }

        [Test]
        public void ShouldSetAttachedGarageSqFt()
        {
            var polkCountyHouseData = PolkCountyHouseData.CreateFrom(_realEstateRecord);

            Assert.AreEqual(_realEstateRecord.Residence.AttachedGarageSquareFootage, polkCountyHouseData.GarageSqFt);
        }

        [Test]
        public void ShouldSetBasementFinished()
        {
            var polkCountyHouseData = PolkCountyHouseData.CreateFrom(_realEstateRecord);

            Assert.AreEqual(_realEstateRecord.Residence.LivingAreaSquareFootage[-1],
                polkCountyHouseData.BasementFinished);
        }

        [Test]
        public void ShouldSetBasementSqFt()
        {
            var polkCountyHouseData = PolkCountyHouseData.CreateFrom(_realEstateRecord);

            Assert.AreEqual(_realEstateRecord.Residence.UnfinishedBasementSquareFootage,
                polkCountyHouseData.BasementSqFt);
        }

        [Test]
        public void ShouldSetBathrooms()
        {
            var polkCountyHouseData = PolkCountyHouseData.CreateFrom(_realEstateRecord);

            Assert.AreEqual(_realEstateRecord.Residence.Bathrooms, polkCountyHouseData.Bathrooms);
        }

        [Test]
        public void ShouldSetBedrooms()
        {
            var polkCountyHouseData = PolkCountyHouseData.CreateFrom(_realEstateRecord);

            Assert.AreEqual(_realEstateRecord.Residence.Bedrooms, polkCountyHouseData.Bedrooms);
        }

        [Test]
        public void ShouldSetCity()
        {
            var polkCountyHouseData = PolkCountyHouseData.CreateFrom(_realEstateRecord);

            Assert.AreEqual(_realEstateRecord.Location.City, polkCountyHouseData.City);
        }

        [Test]
        public void ShouldSetCounty()
        {
            var polkCountyHouseData = PolkCountyHouseData.CreateFrom(_realEstateRecord);

            Assert.AreEqual(_realEstateRecord.Location.County, polkCountyHouseData.County);
        }

        [Test]
        public void ShouldSetLivableSqFt()
        {
            var polkCountyHouseData = PolkCountyHouseData.CreateFrom(_realEstateRecord);

            Assert.AreEqual(_realEstateRecord.Residence.TotalLivingAreaSquareFootage, polkCountyHouseData.LivableSqFt);
        }

        [Test]
        public void ShouldSetLotSize()
        {
            var polkCountyHouseData = PolkCountyHouseData.CreateFrom(_realEstateRecord);

            Assert.AreEqual(_realEstateRecord.Land.SquareFeet, polkCountyHouseData.LotSize);
        }

        [Test]
        public void ShouldSetState()
        {
            var polkCountyHouseData = PolkCountyHouseData.CreateFrom(_realEstateRecord);

            Assert.AreEqual(_realEstateRecord.Location.State, polkCountyHouseData.State);
        }

        [Test]
        public void ShouldSetToiletRooms()
        {
            var polkCountyHouseData = PolkCountyHouseData.CreateFrom(_realEstateRecord);

            Assert.AreEqual(_realEstateRecord.Residence.ToiletRooms, polkCountyHouseData.ToiletRooms);
        }

        [Test]
        public void ShouldSetYearBuilt()
        {
            var polkCountyHouseData = PolkCountyHouseData.CreateFrom(_realEstateRecord);

            Assert.AreEqual(_realEstateRecord.Residence.YearBuilt, polkCountyHouseData.YearBuilt);
        }

        [Test]
        public void ShouldSetZip()
        {
            var polkCountyHouseData = PolkCountyHouseData.CreateFrom(_realEstateRecord);

            Assert.AreEqual(_realEstateRecord.Location.Zip, polkCountyHouseData.Zip);
        }
    }
}