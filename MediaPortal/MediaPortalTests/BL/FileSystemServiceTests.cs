using System;
using System.Collections.Generic;
using MediaPortal.BL.Interface;
using MediaPortal.BL.Models;
using MediaPortal.BL.Services;
using MediaPortal.Data.EntitiesModel;
using MediaPortal.Data.Interface;
using NSubstitute;
using NUnit.Framework;

namespace MediaPortalTests.BL
{
    public class FileSystemServiceTests
    {
        private IFileSystemRepository<FileSystem> _fileSyatemRepository;
        private IFileSystemService _fileSystemService;

        [SetUp]
        public void SetUp()
        {
            _fileSyatemRepository = Substitute.For<IFileSystemRepository<FileSystem>>();
            _fileSystemService = new FileSystemService(_fileSyatemRepository);
        }

        [Test]
        public void GetAllUserFileSystem_Test()
        {
            // Arrange
            string userId = "userId";
            var fileSystems = GetMockFileSystems(userId);

            _fileSyatemRepository.GetAll().Returns(fileSystems);

            // Act
            List<FileSystemDTO> dto = _fileSystemService.GetAllUserFileSystem(userId);

            // Assert
            Assert.AreEqual(fileSystems.Count, dto.Count);

            var expectedFilesystem = fileSystems[0];
            var filesystemDto = dto[0];

            UserFileSystem_Test_Assertion(expectedFilesystem, filesystemDto);
        }

        [Test]
        [TestCase(1)]
        [TestCase(null)]
        public void GetUserFileSystem_Test(int? fileSystemParentId)
        {
            // Arrange
            string userId = "userId";
            var fileSystems = GetMockFileSystems(userId);

            _fileSyatemRepository.GetAll(userId, fileSystemParentId).Returns(fileSystems);

            // Act
            List<FileSystemDTO> dto = _fileSystemService.GetUserFileSystem(userId, fileSystemParentId);

            // Assert
            Assert.AreEqual(fileSystems.Count, dto.Count);

            var expectedFilesystem = fileSystems[0];
            var filesystemDto = dto[0];

            UserFileSystem_Test_Assertion(expectedFilesystem, filesystemDto);
        }

        [Test]
        [TestCase(1)]
        [TestCase(null)]
        public void GetUserFileSystem_Test_Exception(int? fileSystemParentId)
        {
            // Arrange
            string userId = "userId";

            _fileSyatemRepository.GetAll(userId, fileSystemParentId).Returns(x => { throw new Exception(); });

            // Act
            List<FileSystemDTO> dto = _fileSystemService.GetUserFileSystem(userId, fileSystemParentId);

            // TODO: write test for logger
        }

        private List<FileSystem> GetMockFileSystems(string userId)
        {
            return new List<FileSystem>
            {
                new FileSystem
                {
                    BlobLink = "link",
                    BlobThumbnail = "thumbnail",
                    Id=  1,
                    Name = "name",
                    ParentId = null,
                    Size = 123,
                    Tags = new Tag[]
                    {
                        new Tag { Id = 1, Name = "tag1" }
                    },
                    Type = "type",
                    UserId = userId
                }
            };
        }

        private void UserFileSystem_Test_Assertion(FileSystem expectedFilesystem, FileSystemDTO filesystemDto)
        {
            Assert.AreEqual(expectedFilesystem.BlobLink, filesystemDto.BlobLink);
            Assert.AreEqual(expectedFilesystem.BlobThumbnail, filesystemDto.BlobThumbnail);
            Assert.AreEqual(expectedFilesystem.Id, filesystemDto.Id);
            Assert.AreEqual(expectedFilesystem.Name, filesystemDto.Name);
            Assert.AreEqual(expectedFilesystem.ParentId, filesystemDto.ParentId);
            Assert.AreEqual(expectedFilesystem.Size, filesystemDto.Size);
            Assert.AreEqual(expectedFilesystem.Type, filesystemDto.Type);
            Assert.AreEqual(expectedFilesystem.UserId, filesystemDto.UserId);
        }
    }
}
