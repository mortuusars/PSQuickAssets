using PSQuickAssets.Assets;
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
            _sut.AddAsset(new Asset() { Path = "C:\\test.jpg" }, DuplicateHandling.Deny);

            Assert.True(eventOccured);
        }

        [Fact]
        public void GroupChagedShouldRaiseIfMultipleAssetsAdded()
        {
            bool eventOccured = false;

            List<Asset> multiple = new List<Asset>()
            {
                new Asset() { Path = "C:\\test.jpg"},
                new Asset() { Path = "C:\\testing.jpg"},
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
            _sut.Name = "123";
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
            _sut.AddAsset(new Asset() { Path = "C:\\test\\a.jpg" }, DuplicateHandling.Deny);

            Assert.Single(_sut.Assets);
        }

        [Fact]
        public void AddAssetWithDenyShouldReturnFalseIfDuplicateIsPassed()
        {
            _sut.AddAsset(new Asset() { Path = "C:\\test\\a.jpg" }, DuplicateHandling.Deny);

            bool dupResult = _sut.AddAsset(new Asset() { Path = "C:\\test\\a.jpg" }, DuplicateHandling.Deny);

            Assert.False(dupResult);
        }

        [Fact]
        public void AddAssetWithAllowShouldAddIfDuplicateIsPassed()
        {
            _sut.AddAsset(new Asset() { Path = "C:\\test\\a.jpg" }, DuplicateHandling.Allow);

            bool dupResult = _sut.AddAsset(new Asset() { Path = "C:\\test\\a.jpg" }, DuplicateHandling.Allow);

            Assert.True(dupResult);
            Assert.Equal(2, _sut.Assets.Count);
        }

        [Fact]
        public void AddMultipleDenyShouldReturnDuplicateAssets()
        {
            List<Asset> assets = new List<Asset>()
            {
                new Asset() { Path = "C:\\test.jpg"},
                new Asset() { Path = "C:\\test.jpg"},
                new Asset() { Path = "C:\\test1.jpg"},
                new Asset() { Path = "C:\\testing.jpg"},
                new Asset() { Path = "C:\\testing.jpg"},
            };

            List<Asset> duplicates = new List<Asset>()
            {
                new Asset() { Path = "C:\\test.jpg"},
                new Asset() { Path = "C:\\testing.jpg"},
            };

            var returnedDuplicates = _sut.AddMultipleAssets(assets, DuplicateHandling.Deny);
            Assert.Equal(duplicates[0].Path, returnedDuplicates[0].Path);
            Assert.Equal(duplicates[1].Path, returnedDuplicates[1].Path);
        }

        #endregion

        #region Rename

        [Fact]
        public void RenameThrowsIfPassedNull()
        {
            Assert.Throws<ArgumentNullException>(() => _sut.Name = null);
        }

        [Fact]
        public void RenameNotChangesNameIfSame()
        {
            _sut.Name = "same name";
            bool groupChanged = false;
            _sut.GroupChanged += () => groupChanged = true;

            _sut.Name = "same name";

            Assert.False(groupChanged);
        }

        [Fact]
        public void RenameChangesNameIfDifferentCase()
        {
            _sut.Name = "new name";

            _sut.Name = "New name";

            Assert.True(_sut.Name.Equals("New name"));
        }

        [Fact]
        public void RenameShouldChangeGroupName()
        {
            _sut.Name = "Old name";

            _sut.Name = "New name";

            Assert.True(_sut.Name.Equals("New name"));
        }

        #endregion

        #region HasAsset

        [Fact]
        public void HasAssetShouldReturnFalseIfNoneFound()
        {
            _sut.AddAsset(new Asset() { Path = "C:\\test.jpg" }, DuplicateHandling.Deny);
            _sut.AddAsset(new Asset() { Path = "C:\\test1.jpg" }, DuplicateHandling.Deny);

            Assert.False(_sut.HasAsset(new Asset() { Path = "D:\\a.jpg" }));
        }

        [Fact]
        public void HasAssetShouldReturnTrueIfDuplicateFound()
        {
            _sut.AddAsset(new Asset() { Path = "C:\\test.jpg" }, DuplicateHandling.Deny);
            _sut.AddAsset(new Asset() { Path = "C:\\test1.jpg" }, DuplicateHandling.Deny);

            Assert.True(_sut.HasAsset(new Asset() { Path = "C:\\test1.jpg" }));
        }

        [Fact]
        public void HasAssetThrowsNullReferenceIfNullIsPassed()
        {
            _sut.AddAsset(new Asset() { Path = "C:\\test.jpg" }, DuplicateHandling.Deny);
            _sut.AddAsset(new Asset() { Path = "C:\\test1.jpg" }, DuplicateHandling.Deny);

            Assert.Throws<NullReferenceException>(() => _sut.HasAsset(null));
        }

        #endregion
    }
}
