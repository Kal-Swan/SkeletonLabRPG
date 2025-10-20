
from typing import List
from pydantic import BaseModel


class Build(BaseModel):
    name: str
    reason: str
    template: str


class BuildList(BaseModel):
    builds: List[Build]