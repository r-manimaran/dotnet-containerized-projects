using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.DTOs;

public record ServiceResponse(bool IsSuccess =false, string Message =null!);

