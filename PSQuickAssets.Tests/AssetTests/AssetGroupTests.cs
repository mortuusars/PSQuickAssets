using PSQuickAssets.Models;
using System;
using System.Collections.Generic;
using Xunit;

namespace PSQuickAssets.Tests.AssetTests
{
    public class AssetGroupTests
    {
        public AssetGroup _sut = new AssetGroup("test");

        #region Group Changed Event

        [Fact]
        public void GroupChagedShouldRaiseIfAssetAdded()
        {
            bool eventOccured = false;

            _sut.GroupChanged += () => eventOccured = true;
            _sut.AddAsset(new Asset() { FilePath = "C:\\test.jpg" }, DuplicateHandling.Deny);

            Assert.True(eventOccured);
        }

        [Fact]
        public void GroupChagedShouldRaiseIfMultipleAssetsAdded()
        {
            bool eventOccured = false;

            List<Asset> multiple = new List<Asset>()
            {
                new Asset() { FilePath = "C:\\test.jpg"},
                new Asset() { FilePath = "C:\\testing.jpg"},
            };

            _sut.GroupChanged += () => eventOccured = true;
            _sut.AddMultipleAssets(multiple, DuplicateHandling.Deny);

            Assert.True(eventOccured);
        }

        [Fact]
        public void GroupChagedShouldRaiseIfGroupRenamed()
        {
            bool eventOccured = false;

            _sut.GroupChanged += () => eventOccured = true;
            _sut.Rename("123");
            Assert.True(eventOccured);
        }

        [Fact]
        public void GroupChangedShouldRaiseIfAssetCollectionIsChanged()
        {
            bool eventOccured = false;

            _sut.GroupChanged += () => eventOccured = true;

            _sut.Assets.Clear();

            Assert.True(eventOccured);
        }

        #endregion

        #region AddAssets

        [Fact]
        public void GroupShouldHaveNameThatWasSetOnCreation()
        {
            Assert.Equal("test", _sut.Name);
        }

        [Fact]
        public void AssetGroupShouldThrowWhenNullIsPassedAsName()
        {
            Assert.Throws<ArgumentNullException>(() => new AssetGroup(null));
        }

        [Fact]
        public void AddAssetShouldAddAssetToGroup()
        {
            _sut.AddAsset(new Asset() { FilePath = "C:\\test\\a.jpg" }, DuplicateHandling.Deny);

            Assert.Single(_sut.Assets);
        }

        [Fact]
        public void AddAssetWithDenyShouldReturnFalseIfDuplicateIsPassed()
        {
            _sut.AddAsset(new Asset() { FilePath = "C:\\test\\a.jpg" }, DuplicateHandling.Deny);

            bool dupResult = _sut.AddAsset(new Asset() { FilePath = "C:\\test\\a.jpg" }, DuplicateHandling.Deny);

            Assert.False(dupResult);
        }

        [Fact]
        public void AddAssetWithAllowShouldAddIfDuplicateIsPassed()
        {
            _sut.AddAsset(new Asset() { FilePath = "C:\\test\\a.jpg" }, DuplicateHandling.Allow);

            bool dupResult = _sut.AddAsset(new Asset() { FilePath = "C:\\test\\a.jpg" }, DuplicateHandling.Allow);

            Assert.True(dupResult);
            Assert.Equal(2, _sut.Assets.Count);
        }

        [Fact]
        public void AddMultipleDenyShouldReturnDuplicateAssets()
        {
            List<Asset> assets = new List<Asset>()
            {
                new Asset() { FilePath = "C:\\test.jpg"},
                new Asset() { FilePath = "C:\\test.jpg"},
                new Asset() { FilePath = "C:\\test1.jpg"},
                new Asset() { FilePath = "C:\\testing.jpg"},
                new Asset() { FilePath = "C:\\testing.jpg"},
            };

            List<Asset> duplicates = new List<Asset>()
            {
                new Asset() { FilePath = "C:\\test.jpg"},
                new Asset() { FilePath = "C:\\testing.jpg"},
            };

            var returnedDuplicates = _sut.AddMultipleAssets(assets, DuplicateHandling.Deny);
            Assert.Equal(duplicates[0].FilePath, returnedDuplicates[0].FilePath);
            Assert.Equal(duplicates[1].FilePath, returnedDuplicates[1].FilePath);
        }

        #endregion

        #region Rename

        [Fact]
        public void RenameThrowsIfPassedNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Rename(null));
        }

        [Fact]
        public void RenameNotChangesNameIfSame()
        {
            Assert.False(_sut.Rename("test"));
        }

        [Fact]
        public void RenameChangesNameIfDifferentCase()
        {
            Assert.True(_sut.Rename("Test"));
        }

        [Fact]
        public void RenameShouldChangeGroupName()
        {
            _sut.Rename("changed");

            Assert.Equal("changed", _sut.Name);
        }

        #endregion

        #region HasAsset

        [Fact]
        public void HasAssetShouldReturnFalseIfNoneFound()
        {
            _sut.AddAsset(new Asset() { FilePath = "C:\\test.jpg" }, DuplicateHandling.Deny);
            _sut.AddAsset(new Asset() { FilePath = "C:\\test1.jpg" }, DuplicateHandling.Deny);

            Assert.False(_sut.HasAsset(new Asset() { FilePath = "D:\\a.jpg" }));
        }

        [Fact]
        public void HasAssetShouldReturnTrueIfDuplicateFound()
        {
            _sut.AddAsset(new Asset() { FilePath = "C:\\test.jpg" }, DuplicateHandling.Deny);
            _sut.AddAsset(new Asset() { FilePath = "C:\\test1.jpg" }, DuplicateHandling.Deny);

            Assert.True(_sut.HasAsset(new Asset() { FilePath = "C:\\test1.jpg" }));
        }

        [Fact]
        public void HasAssetThrowsNullReferenceIfNullIsPassed()
        {
            _sut.AddAsset(new Asset() { FilePath = "C:\\test.jpg" }, DuplicateHandling.Deny);
            _sut.AddAsset(new Asset() { FilePath = "C:\\test1.jpg" }, DuplicateHandling.Deny);

            Assert.Throws<NullReferenceException>(() => _sut.HasAsset(null));
        }

        #endregion
    }
}
