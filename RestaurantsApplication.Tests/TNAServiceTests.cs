using Moq;
using RestaurantsApplication.Data.Enums;
using RestaurantsApplication.DTOs.DatabaseCopiesDTOs;
using RestaurantsApplication.DTOs.ShiftDTOs;
using RestaurantsApplication.Repositories.Contracts;
using RestaurantsApplication.Services.RecordValidator;
using RestaurantsApplication.Services.Services;

namespace RestaurantsApplication.Tests
{
    public class TNAServiceTests
    {
        private TNAService _service;
        private Mock<IRequestRepository> _requestRepository;
        private Mock<IShiftRepository> _shiftRepository;
        private Mock<IEmployeeRepository> _employeeRepository;
        private Mock<IRecordValidator> _recordValidator;

        [SetUp]
        public void Setup()
        {
            _requestRepository = new Mock<IRequestRepository>();
            _shiftRepository = new Mock<IShiftRepository>();
            _employeeRepository = new Mock<IEmployeeRepository>();
            _recordValidator = new Mock<IRecordValidator>();

            _requestRepository.Setup(r => r.PendingRequestsExist()).Returns(true);

            _service = new TNAService(_requestRepository.Object, _shiftRepository.Object, _employeeRepository.Object, _recordValidator.Object);
        }

        [Test]
        public async Task ServiceCreatesShiftsWithKnownRole_WhenDataIsValid_AndEmploymentIsSingle()
        {
            //Arange
            List<RequestCopyDTO> requests = new List<RequestCopyDTO>()
            {
                new RequestCopyDTO
                {
                    Date = new DateTime(2023,01,25),
                    LocationCode = "LocationCode",
                    Records = new List<RecordCopyDTO>()
                    {
                        new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = 0,
                            ClockValue = new DateTime(2023,01,25,10,0,0)
                        },
                         new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = (ClockStatus)1,
                            ClockValue = new DateTime(2023,01,25,12,0,0)
                        },
                          new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = (ClockStatus)2,
                            ClockValue = new DateTime(2023,01,25,13,0,0)
                        },
                         new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = (ClockStatus)3,
                            ClockValue = new DateTime(2023,01,25,19,0,0),
                        },
                          new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = 0,
                            ClockValue = new DateTime(2023,01,25,19,0,0)
                        },
                             new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = (ClockStatus)3,
                            ClockValue = new DateTime(2023,01,25,20,0,0)
                        }
                    }
                }
            };

            EmployeeCopyDTO employee = new EmployeeCopyDTO
            {
                Code = "EmployeeCode",
                Employments = new List<EmploymentCopyDTO>
                {
                    new EmploymentCopyDTO
                    {
                        DepartmentId = 1,
                        RoleId = 1,
                        Department = new DepartmentCopyDTO
                        {
                            Location = new LocationCopyDTO
                            {
                                Code = "LocationCode"
                            }
                        }
                    }
                }
            };

            _requestRepository.Setup(r => r.GetRequestsAndSetStatus()).ReturnsAsync(requests);

            _employeeRepository.Setup(e => e.GetByCodeWithIncludesAsync("EmployeeCode")).ReturnsAsync(employee);

            List<ShiftCopyDTO> shifts = new List<ShiftCopyDTO>();

            _shiftRepository.Setup(s => s.AddRange(It.IsAny<List<ShiftCopyDTO>>())).Callback((List<ShiftCopyDTO> s) => { shifts = s; });

            //Act
            await _service.ProcessRequestsAsync();

            //Assert
            Assert.That(shifts.Count == 2);

            var shift1 = shifts[0];

            Assert.That(shift1.Start == new DateTime(2023, 01, 25, 10, 0, 0));
            Assert.That(shift1.BreakStart == new DateTime(2023, 01, 25, 12, 0, 0));
            Assert.That(shift1.BreakEnd == new DateTime(2023, 01, 25, 13, 0, 0));
            Assert.That(shift1.End == new DateTime(2023, 01, 25, 19, 0, 0));
            Assert.That(shift1.RoleId == 1);
            Assert.That(shift1.DepartmentId == 1);

            var shift2 = shifts[1];

            Assert.That(shift2.Start == new DateTime(2023, 01, 25, 19, 0, 0));
            Assert.That(shift2.End == new DateTime(2023, 01, 25, 20, 0, 0));
            Assert.That(shift2.RoleId == 1);
            Assert.That(shift2.DepartmentId == 1);

            Assert.That(requests[0].Status == "Completed");
        }

        [Test]
        public async Task ServiceCreatesShiftWithoutRole_WhenDataIsValid_AndEmploymentsAreMultiple()
        {
            //Arange
            List<RequestCopyDTO> requests = new List<RequestCopyDTO>()
            {
                new RequestCopyDTO
                {
                    Date = new DateTime(2023,01,25),
                    LocationCode = "LocationCode",
                    Records = new List<RecordCopyDTO>()
                    {
                        new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = 0,
                            ClockValue = new DateTime(2023,01,25,10,0,0)
                        },
                         new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = (ClockStatus)3,
                            ClockValue = new DateTime(2023,01,25,19,0,0),
                        }
                    }
                }
            };

            EmployeeCopyDTO employee = new EmployeeCopyDTO
            {
                Code = "EmployeeCode",
                Employments = new List<EmploymentCopyDTO>
                {
                    new EmploymentCopyDTO
                    {
                        DepartmentId = 1,
                        RoleId = 1,
                        Department = new DepartmentCopyDTO
                        {
                            Location = new LocationCopyDTO
                            {
                                Code = "LocationCode"
                            }
                        }
                    },
                      new EmploymentCopyDTO
                    {
                        DepartmentId = 1,
                        RoleId = 2,
                        Department = new DepartmentCopyDTO
                        {
                            Location = new LocationCopyDTO
                            {
                                Code = "LocationCode"
                            }
                        }
                    }
                }
            };

            _requestRepository.Setup(r => r.GetRequestsAndSetStatus()).ReturnsAsync(requests);

            _employeeRepository.Setup(e => e.GetByCodeWithIncludesAsync("EmployeeCode")).ReturnsAsync(employee);

            List<ShiftCopyDTO> shifts = new List<ShiftCopyDTO>();

            _shiftRepository.Setup(s => s.AddRange(It.IsAny<List<ShiftCopyDTO>>())).Callback((List<ShiftCopyDTO> s) => { shifts = s; });

            //Act
            await _service.ProcessRequestsAsync();

            //Assert
            Assert.That(shifts.Count == 1);

            var shift = shifts[0];

            Assert.That(shift.Start == new DateTime(2023, 01, 25, 10, 0, 0));
            Assert.That(shift.End == new DateTime(2023, 01, 25, 19, 0, 0));
            Assert.That(shift.RoleId == null);
            Assert.That(shift.DepartmentId == null);

            Assert.That(requests[0].Status == "Completed");
        }

        [Test]
        public async Task ServiceDoesNotCreateShift_WhenRecordsAreInvalid()
        {
            //Arange
            List<RequestCopyDTO> requests = new List<RequestCopyDTO>()
            {
                new RequestCopyDTO
                {
                    Date = new DateTime(2023,01,25),
                    LocationCode = "LocationCode",
                    Records = new List<RecordCopyDTO>()
                    {
                        new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = 0,
                            ClockValue = new DateTime(2023,01,25,10,0,0)
                        },
                         new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = (ClockStatus)3,
                            ClockValue = new DateTime(2023,01,25,19,0,0),
                        }
                    }
                }
            };

            EmployeeCopyDTO employee = new EmployeeCopyDTO
            {
                Code = "EmployeeCode",
                Employments = new List<EmploymentCopyDTO>
                {
                    new EmploymentCopyDTO
                    {
                        DepartmentId = 1,
                        RoleId = 1,
                        Department = new DepartmentCopyDTO
                        {
                            Location = new LocationCopyDTO
                            {
                                Code = "LocationCode"
                            }
                        }
                    }
                }
            };

            _recordValidator.Setup(r => r.ValidateRecords(It.IsAny<RequestCopyDTO>(), ref It.Ref<bool>.IsAny, It.IsAny<Dictionary<string, int>>()))
                .Callback((RequestCopyDTO r, ref bool b, Dictionary<string, int> dic) => { b = true; });

            _requestRepository.Setup(r => r.GetRequestsAndSetStatus()).ReturnsAsync(requests);

            _employeeRepository.Setup(e => e.GetByCodeWithIncludesAsync("EmployeeCode")).ReturnsAsync(employee);

            List<ShiftCopyDTO> shifts = new List<ShiftCopyDTO>();

            _shiftRepository.Setup(s => s.AddRange(It.IsAny<List<ShiftCopyDTO>>())).Callback((List<ShiftCopyDTO> s) => { shifts = s; });

            //Act
            await _service.ProcessRequestsAsync();

            //Assert
            Assert.That(requests[0].Status == "Failed");
            Assert.That(shifts.Count == 0);
        }

        [Test]
        public async Task ServiceDoesNotCreateDoubledShifts_WhenDataInRequestIsDoubled()
        {
            //Arange
            List<RequestCopyDTO> requests = new List<RequestCopyDTO>()
            {
                new RequestCopyDTO
                {
                    Date = new DateTime(2023,01,25),
                    LocationCode = "LocationCode",
                    Records = new List<RecordCopyDTO>()
                    {
                        new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = 0,
                            ClockValue = new DateTime(2023,01,25,10,0,0)
                        },
                         new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = (ClockStatus)3,
                            ClockValue = new DateTime(2023,01,25,19,0,0),
                        },
                          new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = 0,
                            ClockValue = new DateTime(2023,01,25,10,0,0)
                        },
                         new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = (ClockStatus)3,
                            ClockValue = new DateTime(2023,01,25,19,0,0),
                        }
                    }
                }
            };

            EmployeeCopyDTO employee = new EmployeeCopyDTO
            {
                Code = "EmployeeCode",
                Employments = new List<EmploymentCopyDTO>
                {
                    new EmploymentCopyDTO
                    {
                        DepartmentId = 1,
                        RoleId = 1,
                        Department = new DepartmentCopyDTO
                        {
                            Location = new LocationCopyDTO
                            {
                                Code = "LocationCode"
                            }
                        }
                    }
                }
            };

            _requestRepository.Setup(r => r.GetRequestsAndSetStatus()).ReturnsAsync(requests);

            _employeeRepository.Setup(e => e.GetByCodeWithIncludesAsync("EmployeeCode")).ReturnsAsync(employee);

            List<ShiftCopyDTO> shifts = new List<ShiftCopyDTO>();

            _shiftRepository.Setup(s => s.AddRange(It.IsAny<List<ShiftCopyDTO>>())).Callback((List<ShiftCopyDTO> s) => { shifts = s; });

            //Act
            await _service.ProcessRequestsAsync();

            //Assert
            Assert.That(shifts.Count == 1);

            var shift = shifts[0];

            Assert.That(shift.Start == new DateTime(2023, 01, 25, 10, 0, 0));
            Assert.That(shift.End == new DateTime(2023, 01, 25, 19, 0, 0));
            Assert.That(shift.RoleId == 1);
            Assert.That(shift.DepartmentId == 1);

            Assert.That(requests[0].Status == "Completed");
        }

        [Test]
        public async Task ServiceDoesNotCreateNewShift_WhenDataInRequestIsEqualToShiftInDatabase()
        {
            //Arange
            List<RequestCopyDTO> requests = new List<RequestCopyDTO>()
            {
                new RequestCopyDTO
                {
                    Date = new DateTime(2023,01,25),
                    LocationCode = "LocationCode",
                    Records = new List<RecordCopyDTO>()
                    {
                        new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = 0,
                            ClockValue = new DateTime(2023,01,25,10,0,0)
                        },
                         new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = (ClockStatus)3,
                            ClockValue = new DateTime(2023,01,25,19,0,0),
                        }
                    }
                }
            };

            EmployeeCopyDTO employee = new EmployeeCopyDTO
            {
                Code = "EmployeeCode",
                Employments = new List<EmploymentCopyDTO>
                {
                    new EmploymentCopyDTO
                    {
                        DepartmentId = 1,
                        RoleId = 1,
                        Department = new DepartmentCopyDTO
                        {
                            Location = new LocationCopyDTO
                            {
                                Code = "LocationCode"
                            }
                        }
                    }
                }
            };

            _requestRepository.Setup(r => r.GetRequestsAndSetStatus()).ReturnsAsync(requests);

            _employeeRepository.Setup(e => e.GetByCodeWithIncludesAsync("EmployeeCode")).ReturnsAsync(employee);

            List<ShiftCopyDTO> shifts = new List<ShiftCopyDTO>();

            _shiftRepository.Setup(s => s.AddRange(It.IsAny<List<ShiftCopyDTO>>())).Callback((List<ShiftCopyDTO> s) => { shifts = s; });

            _shiftRepository.Setup(s => s.CheckEqualShiftsAsync(It.IsAny<ShiftCopyDTO>())).Returns(true);

            //Act
            await _service.ProcessRequestsAsync();

            //Assert
            Assert.That(shifts.Count == 0);

            Assert.That(requests[0].Status == "Completed");
        }

        [Test]
        public async Task ServiceAddsBreaksToShift_WhenShiftDoesNotHaveBreaks_AndBreaksFit()
        {
            //Arange
            var requests = new List<RequestCopyDTO>
            {
                new RequestCopyDTO
                {
                    Date = new DateTime(2023, 01, 25),
                    LocationCode = "LocationCode",
                    Records = new List<RecordCopyDTO>()
                    {
                        new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = (ClockStatus)1,
                            ClockValue = new DateTime(2023,01,25,12,0,0)
                        },
                        new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = (ClockStatus)2,
                            ClockValue = new DateTime(2023,01,25,13,0,0)
                        }
                    }
                }
            };

            EmployeeCopyDTO employee = new EmployeeCopyDTO
            {
                Code = "EmployeeCode",
                Employments = new List<EmploymentCopyDTO>
                {
                    new EmploymentCopyDTO
                    {
                        DepartmentId = 1,
                        RoleId = 1,
                        Department = new DepartmentCopyDTO
                        {
                            Location = new LocationCopyDTO
                            {
                                Code = "LocationCode"
                            }
                        }
                    }
                }
            };

            var shift = new ShiftWithTimesDTO
            {
                Start = new DateTime(2023, 01, 25, 10, 0, 0),
                End = new DateTime(2023, 01, 25, 19, 0, 0),
            };

            _shiftRepository.Setup(s => s.GetShiftWithoutBreakStartAsync(It.IsAny<string>(), It.IsAny<DateTime>())).ReturnsAsync(shift);
            _shiftRepository.Setup(s => s.GetShiftWithoutBreakEndAsync(It.IsAny<string>(), It.IsAny<DateTime>())).ReturnsAsync(shift);

            _requestRepository.Setup(r => r.GetRequestsAndSetStatus()).ReturnsAsync(requests);

            _employeeRepository.Setup(e => e.GetByCodeWithIncludesAsync("EmployeeCode")).ReturnsAsync(employee);

            //Act
            await _service.ProcessRequestsAsync();

            //Assert
            Assert.That(shift.BreakStart == new DateTime(2023, 01, 25, 12, 0, 0));
            Assert.That(shift.BreakEnd == new DateTime(2023, 01, 25, 13, 0, 0));
        }

        [Test]
        public async Task ServiceAddsShifts_WhenOverlapped()
        {
            //Arange
            List<RequestCopyDTO> requests = new List<RequestCopyDTO>()
            {
                new RequestCopyDTO
                {
                    Date = new DateTime(2023,01,25),
                    LocationCode = "LocationCode",
                    Records = new List<RecordCopyDTO>()
                    {
                        new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = 0,
                            ClockValue = new DateTime(2023,01,25,10,0,0)
                        },
                         new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = (ClockStatus)3,
                            ClockValue = new DateTime(2023,01,25,19,0,0),
                        },
                           new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = (ClockStatus)1,
                            ClockValue = new DateTime(2023,01,25,12,0,0),
                        },
                             new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = (ClockStatus)2,
                            ClockValue = new DateTime(2023,01,25,13,0,0),
                        },
                          new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = 0,
                            ClockValue = new DateTime(2023,01,25,15,0,0)
                        },
                             new RecordCopyDTO
                        {
                            EmployeeCode = "EmployeeCode",
                            ClockStatus = (ClockStatus)3,
                            ClockValue = new DateTime(2023,01,25,18,0,0)
                        }
                    }
                }
            };

            EmployeeCopyDTO employee = new EmployeeCopyDTO
            {
                Code = "EmployeeCode",
                Employments = new List<EmploymentCopyDTO>
                {
                    new EmploymentCopyDTO
                    {
                        DepartmentId = 1,
                        RoleId = 1,
                        Department = new DepartmentCopyDTO
                        {
                            Location = new LocationCopyDTO
                            {
                                Code = "LocationCode"
                            }
                        }
                    }
                }
            };

            _requestRepository.Setup(r => r.GetRequestsAndSetStatus()).ReturnsAsync(requests);

            _employeeRepository.Setup(e => e.GetByCodeWithIncludesAsync("EmployeeCode")).ReturnsAsync(employee);

            List<ShiftCopyDTO> shifts = new List<ShiftCopyDTO>();

            _shiftRepository.Setup(s => s.AddRange(It.IsAny<List<ShiftCopyDTO>>())).Callback((List<ShiftCopyDTO> s) => { shifts = s; });

            //Act
            await _service.ProcessRequestsAsync();

            //Assert
            Assert.That(shifts.Count == 2);

            var shift1 = shifts[0];

            Assert.That(shift1.Start == new DateTime(2023, 01, 25, 10, 0, 0));
            Assert.That(shift1.BreakStart == new DateTime(2023, 01, 25, 12, 0, 0));
            Assert.That(shift1.BreakEnd == new DateTime(2023, 01, 25, 13, 0, 0));
            Assert.That(shift1.End == new DateTime(2023, 01, 25, 19, 0, 0));
            Assert.That(shift1.RoleId == 1);
            Assert.That(shift1.DepartmentId == 1);

            var shift2 = shifts[1];

            Assert.That(shift2.Start == new DateTime(2023, 01, 25, 15, 0, 0));
            Assert.That(shift2.End == new DateTime(2023, 01, 25, 18, 0, 0));
            Assert.That(shift2.RoleId == 1);
            Assert.That(shift2.DepartmentId == 1);

            Assert.That(requests[0].Status == "Completed");
        }
    }
}