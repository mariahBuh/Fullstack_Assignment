using MariahBuhagiat_SectionA.Models;
using MariahBuhagiat_SectionA.Services;  
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace MariahBuhagiat_SectionA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapController : ControllerBase
    {
        private static Map _currentMap = new Map();
        private readonly PathFinder _pathFinder;  

        public MapController(PathFinder pathFinder)
        {
            _pathFinder = pathFinder;
        }

        // POST: api/map/SetMap
        [HttpPost("SetMap")]
        public IActionResult SetMap([FromBody] Map map, [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            // API key checks
            if (string.IsNullOrEmpty(apiKey))
                return Unauthorized("API key missing");

            if (apiKey != "FS_ReadWrite")
                return Unauthorized("Invalid API key");

            // Map validation checks
            if (map == null ||
                map.Nodes == null || map.Nodes.Count == 0 ||
                map.Edges == null || map.Edges.Count == 0)
            {
                return BadRequest("Invalid/Missing map data");
            }

            _currentMap = map;
            return Ok("Map Data is stored successfully");
        }


        // GET: api/map/GetMap
        [HttpGet("GetMap")]
        public IActionResult GetMap([FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            // Unauthorized 
            if (string.IsNullOrEmpty(apiKey))
                return Unauthorized("API key missing");

            if (apiKey != "FS_Read")
                return Unauthorized("Invalid API key");

            // Missing Map Data
            if (_currentMap == null || _currentMap.Nodes == null || _currentMap.Nodes.Count == 0)
                return BadRequest("Map has not been set");

            return Ok(_currentMap);
        }


        // GET: /api/map/ShortestRoute?from=G&to=E
        [HttpGet("ShortestRoute")]
        public IActionResult ShortestRoute(string from, string to, [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            // Unauthorized checks
            if (string.IsNullOrEmpty(apiKey))
                return Unauthorized("API key missing");

            if (apiKey != "FS_Read")
                return Unauthorized("Invalid API key");

            // Missing Map Data
            if (_currentMap == null || _currentMap.Nodes == null || _currentMap.Nodes.Count == 0)
                return BadRequest("Map has not been set");

            // Missing parameters
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
                return BadRequest("Missing parameters");

            // Unknown nodes 
            if (!_currentMap.Nodes.Any(n => n.Id == from) ||
                !_currentMap.Nodes.Any(n => n.Id == to))
                return BadRequest("Unknown node");

            var result = _pathFinder.GetShortestPath(_currentMap, from, to);

            // No route found
            if (result.distance == -1)
                return BadRequest("No route found");

            return Ok(result.path);
        }

        // GET: /api/map/ShortestDistance?from=G&to=E
        [HttpGet("ShortestDistance")]
        public IActionResult ShortestDistance(string from, string to, [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            // Unauthorized checks
            if (string.IsNullOrEmpty(apiKey))
                return Unauthorized("API key missing");

            if (apiKey != "FS_Read")
                return Unauthorized("Invalid API key or insufficient permissions");

            // Missing Map Data
            if (_currentMap == null || _currentMap.Nodes == null || _currentMap.Nodes.Count == 0)
                return BadRequest("Map has not been set");

            // Missing parameters
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
                return BadRequest("Missing parameters");

            // Unknown nodes 
            if (!_currentMap.Nodes.Any(n => n.Id == from) ||
                !_currentMap.Nodes.Any(n => n.Id == to))
                return BadRequest("Unknown node");

            var result = _pathFinder.GetShortestPath(_currentMap, from, to);

            // No route found
            if (result.distance == -1)
                return BadRequest("No route found");

            return Ok(result.distance);
        }
    }
}
